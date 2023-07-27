/*
 *  Unreal Engine .NET 6 integration
 *  Copyright (c) 2021 Stanislav Denisov
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

#include "UnrealCLR.h"

#define LOCTEXT_NAMESPACE "UnrealCLR"

DEFINE_LOG_CATEGORY(LogUnrealCLR);

void UnrealCLR::Module::StartupModule() {
	#define HOSTFXR_VERSION "6.0.1"
	#define HOSTFXR_WINDOWS "hostfxr.dll"
	#define HOSTFXR_MAC "libhostfxr.dylib"
	#define HOSTFXR_LINUX "libhostfxr.so"

	#ifdef UNREALCLR_WINDOWS
		#define HOSTFXR_PATH "Plugins/UnrealCLR/Runtime/Win64/host/fxr/" HOSTFXR_VERSION "/" HOSTFXR_WINDOWS
		#define UNREALCLR_PLATFORM_STRING(string) string
	#elif defined(UNREALCLR_MAC)
		#define HOSTFXR_PATH "Plugins/UnrealCLR/Runtime/Mac/host/fxr/" HOSTFXR_VERSION "/" HOSTFXR_MAC
		#define UNREALCLR_PLATFORM_STRING(string) TCHAR_TO_ANSI(string)
	#elif defined(UNREALCLR_UNIX)
		#define HOSTFXR_PATH "Plugins/UnrealCLR/Runtime/Linux/host/fxr/" HOSTFXR_VERSION "/" HOSTFXR_LINUX
		#define UNREALCLR_PLATFORM_STRING(string) TCHAR_TO_ANSI(string)
	#else
		#error "Unknown platform"
	#endif

	UnrealCLR::Status = UnrealCLR::StatusType::Stopped;
	UnrealCLR::ProjectPath = FPaths::ConvertRelativePathToFull(FPaths::ProjectDir());
	UnrealCLR::UserAssembliesPath = UnrealCLR::ProjectPath + TEXT("Managed/");

	OnWorldPostInitializationHandle = FWorldDelegates::OnPostWorldInitialization.AddRaw(this, &UnrealCLR::Module::OnWorldPostInitialization);
	OnWorldCleanupHandle = FWorldDelegates::OnWorldCleanup.AddRaw(this, &UnrealCLR::Module::OnWorldCleanup);

	const FString hostfxrPath = UnrealCLR::ProjectPath + TEXT(HOSTFXR_PATH);
	const FString assembliesPath = UnrealCLR::ProjectPath + TEXT("Plugins/UnrealCLR/Managed/");
	const FString runtimeConfigPath = assembliesPath + TEXT("UnrealEngine.Runtime.runtimeconfig.json");
	const FString runtimeAssemblyPath = assembliesPath + TEXT("UnrealEngine.Runtime.dll");
	const FString runtimeTypeName = TEXT("UnrealEngine.Runtime.Core, UnrealEngine.Runtime");
	const FString runtimeMethodName = TEXT("ManagedCommand");

	UE_LOG(LogUnrealCLR, Display, TEXT("%s: Host path set to \"%s\""), ANSI_TO_TCHAR(__FUNCTION__), *hostfxrPath);

	HostfxrLibrary = FPlatformProcess::GetDllHandle(*hostfxrPath);

	if (HostfxrLibrary) {
		UE_LOG(LogUnrealCLR, Display, TEXT("%s: Host library loaded successfuly!"), ANSI_TO_TCHAR(__FUNCTION__));

		hostfxr_set_error_writer_fn HostfxrSetErrorWriter = (hostfxr_set_error_writer_fn)FPlatformProcess::GetDllExport(HostfxrLibrary, TEXT("hostfxr_set_error_writer"));

		if (!HostfxrSetErrorWriter) {
			UE_LOG(LogUnrealCLR, Error, TEXT("%s: Unable to locate hostfxr_set_error_writer entry point!"), ANSI_TO_TCHAR(__FUNCTION__));

			return;
		}

		hostfxr_initialize_for_runtime_config_fn HostfxrInitializeForRuntimeConfig = (hostfxr_initialize_for_runtime_config_fn)FPlatformProcess::GetDllExport(HostfxrLibrary, TEXT("hostfxr_initialize_for_runtime_config"));

		if (!HostfxrInitializeForRuntimeConfig) {
			UE_LOG(LogUnrealCLR, Error, TEXT("%s: Unable to locate hostfxr_initialize_for_runtime_config entry point!"), ANSI_TO_TCHAR(__FUNCTION__));

			return;
		}

		hostfxr_get_runtime_delegate_fn HostfxrGetRuntimeDelegate = (hostfxr_get_runtime_delegate_fn)FPlatformProcess::GetDllExport(HostfxrLibrary, TEXT("hostfxr_get_runtime_delegate"));

		if (!HostfxrGetRuntimeDelegate) {
			UE_LOG(LogUnrealCLR, Error, TEXT("%s: Unable to locate hostfxr_get_runtime_delegate entry point!"), ANSI_TO_TCHAR(__FUNCTION__));

			return;
		}

		hostfxr_close_fn HostfxrClose = (hostfxr_close_fn)FPlatformProcess::GetDllExport(HostfxrLibrary, TEXT("hostfxr_close"));

		if (!HostfxrClose) {
			UE_LOG(LogUnrealCLR, Error, TEXT("%s: Unable to locate hostfxr_close entry point!"), ANSI_TO_TCHAR(__FUNCTION__));

			return;
		}

		HostfxrSetErrorWriter(&HostError);

		hostfxr_handle HostfxrContext = nullptr;

		if (HostfxrInitializeForRuntimeConfig(UNREALCLR_PLATFORM_STRING(*runtimeConfigPath), nullptr, &HostfxrContext) != 0 || !HostfxrContext) {
			UE_LOG(LogUnrealCLR, Error, TEXT("%s: Unable to initialize the host! Please, try to restart the engine."), ANSI_TO_TCHAR(__FUNCTION__));

			HostfxrClose(HostfxrContext);

			return;
		}

		void* hostfxrLoadAssemblyAndGetFunctionPointer = nullptr;

		if (HostfxrGetRuntimeDelegate(HostfxrContext, hdt_load_assembly_and_get_function_pointer, &hostfxrLoadAssemblyAndGetFunctionPointer) != 0 || !HostfxrGetRuntimeDelegate) {
			UE_LOG(LogUnrealCLR, Error, TEXT("%s: Unable to get hdt_load_assembly_and_get_function_pointer runtime delegate!"), ANSI_TO_TCHAR(__FUNCTION__));

			HostfxrClose(HostfxrContext);

			return;
		}

		HostfxrClose(HostfxrContext);

		UE_LOG(LogUnrealCLR, Display, TEXT("%s: Host functions loaded successfuly!"), ANSI_TO_TCHAR(__FUNCTION__));

		load_assembly_and_get_function_pointer_fn HostfxrLoadAssemblyAndGetFunctionPointer = (load_assembly_and_get_function_pointer_fn)hostfxrLoadAssemblyAndGetFunctionPointer;

		if (HostfxrLoadAssemblyAndGetFunctionPointer && HostfxrLoadAssemblyAndGetFunctionPointer(UNREALCLR_PLATFORM_STRING(*runtimeAssemblyPath), UNREALCLR_PLATFORM_STRING(*runtimeTypeName), UNREALCLR_PLATFORM_STRING(*runtimeMethodName), UNMANAGEDCALLERSONLY_METHOD, nullptr, (void**)&UnrealCLR::ManagedCommand) == 0) {
			UE_LOG(LogUnrealCLR, Display, TEXT("%s: Host runtime assembly loaded successfuly!"), ANSI_TO_TCHAR(__FUNCTION__));
		} else {
			UE_LOG(LogUnrealCLR, Error, TEXT("%s: Host runtime assembly loading failed!"), ANSI_TO_TCHAR(__FUNCTION__));

			return;
		}

		#if WITH_EDITOR
			IPlatformFile& platformFile = FPlatformFileManager::Get().GetPlatformFile();

			if (!platformFile.DirectoryExists(*UnrealCLR::UserAssembliesPath)) {
				platformFile.CreateDirectory(*UnrealCLR::UserAssembliesPath);

				if (!platformFile.DirectoryExists(*UnrealCLR::UserAssembliesPath))
					UE_LOG(LogUnrealCLR, Warning, TEXT("%s: Unable to create a folder for managed assemblies at %s."), ANSI_TO_TCHAR(__FUNCTION__), *UnrealCLR::UserAssembliesPath);
			}
		#endif

		if (UnrealCLR::ManagedCommand) {
			// Framework pointers

			int32 position = 0;
			int32 checksum = 0; //752 --> 8

			{
				int32 head = 0;
				Shared::Functions[position++] = Shared::AssertFunctions;

				Shared::AssertFunctions[head++] = (void*)&UnrealCLRFramework::Assert::OutputMessage;

				checksum += head;
			}

			{
				int32 head = 0;
				Shared::Functions[position++] = Shared::CommandLineFunctions;

				Shared::CommandLineFunctions[head++] = (void*)&UnrealCLRFramework::CommandLine::Get;
				Shared::CommandLineFunctions[head++] = (void*)&UnrealCLRFramework::CommandLine::Set;
				Shared::CommandLineFunctions[head++] = (void*)&UnrealCLRFramework::CommandLine::Append;

				checksum += head;
			}

			{
				int32 head = 0;
				Shared::Functions[position++] = Shared::DebugFunctions;

				Shared::DebugFunctions[head++] = (void*)&UnrealCLRFramework::Debug::Log;
				Shared::DebugFunctions[head++] = (void*)&UnrealCLRFramework::Debug::Exception;
				Shared::DebugFunctions[head++] = (void*)&UnrealCLRFramework::Debug::AddOnScreenMessage;
				Shared::DebugFunctions[head++] = (void*)&UnrealCLRFramework::Debug::ClearOnScreenMessages;

				checksum += head;
			}

			{
				int32 head = 0;
				Shared::Functions[position++] = Shared::ApplicationFunctions;

				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::IsCanEverRender;
				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::IsPackagedForDistribution;
				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::IsPackagedForShipping;
				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::GetProjectDirectory;
				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::GetDefaultLanguage;
				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::GetProjectName;
				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::GetVolumeMultiplier;
				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::SetProjectName;
				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::SetVolumeMultiplier;
				Shared::ApplicationFunctions[head++] = (void*)&UnrealCLRFramework::Application::RequestExit;

				checksum += head;
			}

			checksum += position;

			// Runtime pointers

			Shared::RuntimeFunctions[0] = (void*)&UnrealCLR::Module::Exception;
			Shared::RuntimeFunctions[1] = (void*)&UnrealCLR::Module::Log;

			constexpr void* functions[3] = {
				Shared::RuntimeFunctions,
				Shared::Events,
				Shared::Functions
			};

			if (reinterpret_cast<intptr_t>(UnrealCLR::ManagedCommand(UnrealCLR::Command(functions, checksum))) == 0xF) {
				UE_LOG(LogUnrealCLR, Display, TEXT("%s: Host runtime assembly initialized successfuly!"), ANSI_TO_TCHAR(__FUNCTION__));
			} else {
				UE_LOG(LogUnrealCLR, Error, TEXT("%s: Host runtime assembly initialization failed!"), ANSI_TO_TCHAR(__FUNCTION__));

				return;
			}

			UnrealCLR::Status = UnrealCLR::StatusType::Idle;

			UE_LOG(LogUnrealCLR, Display, TEXT("%s: Host loaded successfuly!"), ANSI_TO_TCHAR(__FUNCTION__));
		} else {
			UE_LOG(LogUnrealCLR, Error, TEXT("%s: Host runtime assembly unable to load the initialization function!"), ANSI_TO_TCHAR(__FUNCTION__));

			return;
		}
	} else {
		UE_LOG(LogUnrealCLR, Error, TEXT("%s: Host library loading failed!"), ANSI_TO_TCHAR(__FUNCTION__));
	}
}

void UnrealCLR::Module::ShutdownModule() {
	FWorldDelegates::OnPostWorldInitialization.Remove(OnWorldPostInitializationHandle);
	FWorldDelegates::OnWorldCleanup.Remove(OnWorldCleanupHandle);

	FPlatformProcess::FreeDllHandle(HostfxrLibrary);
}

void UnrealCLR::Module::OnWorldPostInitialization(UWorld* World, const UWorld::InitializationValues InitializationValues) {
	if (World->IsGameWorld()) {
		if (UnrealCLR::WorldTickState == TickState::Stopped) {
			UnrealCLR::Engine::Manager = NewObject<UUnrealCLRManager>();
			UnrealCLR::Engine::Manager->AddToRoot();
			UnrealCLR::Engine::World = World;

			if (UnrealCLR::Status != UnrealCLR::StatusType::Stopped) {
				UnrealCLR::ManagedCommand(UnrealCLR::Command(CommandType::LoadAssemblies));
				UnrealCLR::Status = UnrealCLR::StatusType::Running;

				for (TActorIterator<AWorldSettings> currentActor(UnrealCLR::Engine::World); currentActor; ++currentActor) {
					RegisterTickFunction(OnPrePhysicsTickFunction, TG_PrePhysics, *currentActor);
					RegisterTickFunction(OnDuringPhysicsTickFunction, TG_DuringPhysics, *currentActor);
					RegisterTickFunction(OnPostPhysicsTickFunction, TG_PostPhysics, *currentActor);
					RegisterTickFunction(OnPostUpdateTickFunction, TG_PostUpdateWork, *currentActor);

					UnrealCLR::WorldTickState = UnrealCLR::TickState::Registered;

					if (UnrealCLR::Shared::Events[OnWorldBegin])
						UnrealCLR::ManagedCommand(UnrealCLR::Command(UnrealCLR::Shared::Events[OnWorldBegin]));

					break;
				}
			} else {
				#if WITH_EDITOR
					FNotificationInfo notificationInfo(FText::FromString(TEXT("UnrealCLR host is not initialized! Please, check logs and try to restart the engine.")));

					notificationInfo.ExpireDuration = 5.0f;

					FSlateNotificationManager::Get().AddNotification(notificationInfo);
				#endif
			}
		}
	}
}

void UnrealCLR::Module::OnWorldCleanup(UWorld* World, bool SessionEnded, bool CleanupResources) {
	if (World->IsGameWorld() && World == UnrealCLR::Engine::World && UnrealCLR::WorldTickState != UnrealCLR::TickState::Stopped) {
		if (UnrealCLR::Status != UnrealCLR::StatusType::Stopped) {
			if (UnrealCLR::Shared::Events[OnWorldEnd])
				UnrealCLR::ManagedCommand(UnrealCLR::Command(UnrealCLR::Shared::Events[OnWorldEnd]));

			OnPrePhysicsTickFunction.UnRegisterTickFunction();
			OnDuringPhysicsTickFunction.UnRegisterTickFunction();
			OnPostPhysicsTickFunction.UnRegisterTickFunction();
			OnPostUpdateTickFunction.UnRegisterTickFunction();

			UnrealCLR::ManagedCommand(UnrealCLR::Command(CommandType::UnloadAssemblies));
			UnrealCLR::Status = UnrealCLR::StatusType::Idle;
		}

		UnrealCLR::Engine::World = nullptr;
		UnrealCLR::Engine::Manager->RemoveFromRoot();
		UnrealCLR::Engine::Manager = nullptr;
		UnrealCLR::WorldTickState = UnrealCLR::TickState::Stopped;

		FMemory::Memset(UnrealCLR::Shared::Events, 0, sizeof(UnrealCLR::Shared::Events));
	}
}

void UnrealCLR::Module::RegisterTickFunction(FTickFunction& TickFunction, ETickingGroup TickGroup, AWorldSettings* LevelActor) {
	TickFunction.bCanEverTick = true;
	TickFunction.bTickEvenWhenPaused = false;
	TickFunction.bStartWithTickEnabled = true;
	TickFunction.bHighPriority = true;
	TickFunction.bAllowTickOnDedicatedServer = true;
	TickFunction.bRunOnAnyThread = false;
	TickFunction.TickGroup = TickGroup;
	TickFunction.RegisterTickFunction(UnrealCLR::Engine::World->PersistentLevel);
	LevelActor->PrimaryActorTick.AddPrerequisite(UnrealCLR::Engine::Manager, TickFunction);
}

void UnrealCLR::Module::HostError(const char_t* Message) {
	UE_LOG(LogUnrealCLR, Error, TEXT("%s: %s"), ANSI_TO_TCHAR(__FUNCTION__), *FString(Message));
}

void UnrealCLR::Module::Exception(const char* Message) {
	const FString message(ANSI_TO_TCHAR(Message));

	FString OutputLog(message);

	OutputLog.ReplaceCharInline(TEXT('\n'), TEXT(' '));
	OutputLog.ReplaceCharInline(TEXT('\r'), TEXT(' '));
	OutputLog.ReplaceInline(TEXT("     "), TEXT(" "));

	UE_LOG(LogUnrealCLR, Error, TEXT("%s: %s"), ANSI_TO_TCHAR(__FUNCTION__), *OutputLog);

	GEngine->AddOnScreenDebugMessage((uint64)-1, 10.0f, FColor::Red, *message);
}

void UnrealCLR::Module::Log(UnrealCLR::LogLevel Level, const char* Message) {
	#define UNREALCLR_LOG(Verbosity) UE_LOG(LogUnrealCLR, Verbosity, TEXT("%s: %s"), ANSI_TO_TCHAR(__FUNCTION__), *message);

	FString message(ANSI_TO_TCHAR(Message));

	if (Level == UnrealCLR::LogLevel::Display) {
		UNREALCLR_LOG(Display);
	} else if (Level == UnrealCLR::LogLevel::Warning) {
		UNREALCLR_LOG(Warning);

		GEngine->AddOnScreenDebugMessage((uint64)-1, 60.0f, FColor::Yellow, *message);
	} else if (Level == UnrealCLR::LogLevel::Error) {
		UNREALCLR_LOG(Error);

		GEngine->AddOnScreenDebugMessage((uint64)-1, 60.0f, FColor::Red, *message);
	} else if (Level == UnrealCLR::LogLevel::Fatal) {
		UNREALCLR_LOG(Error);

		GEngine->AddOnScreenDebugMessage((uint64)-1, 60.0f, FColor::Red, *message);

		UnrealCLR::Status = UnrealCLR::StatusType::Idle;
	}
}

void UnrealCLR::PrePhysicsTickFunction::ExecuteTick(float DeltaTime, enum ELevelTick TickType, ENamedThreads::Type CurrentThread, const FGraphEventRef& MyCompletionGraphEvent) {
	if (UnrealCLR::WorldTickState != UnrealCLR::TickState::Started && UnrealCLR::Shared::Events[OnWorldPostBegin]) {
		UnrealCLR::ManagedCommand(UnrealCLR::Command(UnrealCLR::Shared::Events[OnWorldPostBegin]));
		UnrealCLR::WorldTickState = UnrealCLR::TickState::Started;
	}

	if (UnrealCLR::Shared::Events[OnWorldPrePhysicsTick])
		UnrealCLR::ManagedCommand(UnrealCLR::Command(UnrealCLR::Shared::Events[OnWorldPrePhysicsTick], DeltaTime));
}

void UnrealCLR::DuringPhysicsTickFunction::ExecuteTick(float DeltaTime, enum ELevelTick TickType, ENamedThreads::Type CurrentThread, const FGraphEventRef& MyCompletionGraphEvent) {
	if (UnrealCLR::Shared::Events[OnWorldDuringPhysicsTick])
		UnrealCLR::ManagedCommand(UnrealCLR::Command(UnrealCLR::Shared::Events[OnWorldDuringPhysicsTick], DeltaTime));
}

void UnrealCLR::PostPhysicsTickFunction::ExecuteTick(float DeltaTime, enum ELevelTick TickType, ENamedThreads::Type CurrentThread, const FGraphEventRef& MyCompletionGraphEvent) {
	if (UnrealCLR::Shared::Events[OnWorldPostPhysicsTick])
		UnrealCLR::ManagedCommand(UnrealCLR::Command(UnrealCLR::Shared::Events[OnWorldPostPhysicsTick], DeltaTime));
}

void UnrealCLR::PostUpdateTickFunction::ExecuteTick(float DeltaTime, enum ELevelTick TickType, ENamedThreads::Type CurrentThread, const FGraphEventRef& MyCompletionGraphEvent) {
	if (UnrealCLR::Shared::Events[OnWorldPostUpdateTick])
		UnrealCLR::ManagedCommand(UnrealCLR::Command(UnrealCLR::Shared::Events[OnWorldPostUpdateTick], DeltaTime));
}

FString UnrealCLR::PrePhysicsTickFunction::DiagnosticMessage() {
	return TEXT("PrePhysicsTickFunction");
}

FString UnrealCLR::DuringPhysicsTickFunction::DiagnosticMessage() {
	return TEXT("DuringPhysicsTickFunction");
}

FString UnrealCLR::PostPhysicsTickFunction::DiagnosticMessage() {
	return TEXT("PostPhysicsTickFunction");
}

FString UnrealCLR::PostUpdateTickFunction::DiagnosticMessage() {
	return TEXT("PostUpdateTickFunction");
}

size_t UnrealCLR::Utility::Strcpy(char* Destination, const char* Source, size_t Length) {
	char* destination = Destination;
	const char* source = Source;
	size_t length = Length;

	if (length != 0 && --length != 0) {
		do {
			if ((*destination++ = *source++) == 0)
				break;
		}

		while (--length != 0);
	}

	if (length == 0) {
		if (Length != 0)
			*destination = '\0';

		while (*source++);
	}

	return (source - Source - 1);
}

size_t UnrealCLR::Utility::Strlen(const char* Source) {
	return strlen(Source) + 1;
}

#undef LOCTEXT_NAMESPACE

IMPLEMENT_MODULE(UnrealCLR::Module, UnrealCLR)

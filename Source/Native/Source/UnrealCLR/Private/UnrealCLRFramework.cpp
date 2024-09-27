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

#include "UnrealCLRFramework.h"

DEFINE_LOG_CATEGORY(LogUnrealManaged);


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

namespace UnrealCLRFramework {
	

	#define UNREALCLR_GET_PROPERTY_VALUE(Type, Object, Name, Value)\
		FName name(UTF8_TO_TCHAR(Name));\
		for (TFieldIterator<Type> currentProperty(Object->GetClass()); currentProperty; ++currentProperty) {\
			Type* property = *currentProperty;\
			if (property->GetFName() == name) {\
				*Value = property->GetPropertyValue_InContainer(Object);\
				return true;\
			}\
		}\
		return false;

	#define UNREALCLR_SET_PROPERTY_VALUE(Type, Object, Name, Value)\
		FName name(UTF8_TO_TCHAR(Name));\
		for (TFieldIterator<Type> currentProperty(Object->GetClass()); currentProperty; ++currentProperty) {\
			Type* property = *currentProperty;\
			if (property->GetFName() == name) {\
				property->SetPropertyValue_InContainer(Object, Value);\
				return true;\
			}\
		}\
		return false;


	#define UNREALCLR_COLOR_TO_INTEGER(Color) (Color.A << 24) + (Color.R << 16) + (Color.G << 8) + Color.B

	#if ENGINE_MAJOR_VERSION == 4
		#if ENGINE_MINOR_VERSION <= 26
			#define UNREALCLR_BLEND_TYPE 5
		#elif ENGINE_MINOR_VERSION >= 27
			#define UNREALCLR_BLEND_TYPE 6
		#endif

		#if ENGINE_MINOR_VERSION <= 25
			#define UNREALCLR_PIXEL_FORMAT 71
		#elif ENGINE_MINOR_VERSION >= 26
			#define UNREALCLR_PIXEL_FORMAT 72
		#endif
	#elif ENGINE_MAJOR_VERSION == 5
		#define UNREALCLR_PIXEL_FORMAT 72
		#define UNREALCLR_BLEND_TYPE 6
	#endif

	namespace Assert {
		void OutputMessage(const char* Message) {
			FString message(UTF8_TO_TCHAR(Message));

			UE_LOG(LogUnrealManaged, Error, TEXT("%s: %s"), ANSI_TO_TCHAR(__FUNCTION__), *message);

			GEngine->AddOnScreenDebugMessage((uint64)-1, 60.0f, FColor::Red, *message);
		}
	}

	namespace CommandLine {
		void Get(char* Arguments) {
			const char* arguments = TCHAR_TO_UTF8(FCommandLine::Get());

			UnrealCLR::Utility::Strcpy(Arguments, arguments, UnrealCLR::Utility::Strlen(arguments));
		}

		void Set(const char* Arguments) {
			FCommandLine::Set(UTF8_TO_TCHAR(Arguments));
		}

		void Append(const char* Arguments) {
			FCommandLine::Append(UTF8_TO_TCHAR(Arguments));
		}
	}

	namespace Debug {
		void Log(LogLevel Level, const char* Message) {
			#define UNREALCLR_FRAMEWORK_LOG(Verbosity) UE_LOG(LogUnrealManaged, Verbosity, TEXT("%s: %s"), ANSI_TO_TCHAR(__FUNCTION__), *FString(UTF8_TO_TCHAR(Message)));

			if (Level == LogLevel::Display) {
				UNREALCLR_FRAMEWORK_LOG(Display);
			} else if (Level == LogLevel::Warning) {
				UNREALCLR_FRAMEWORK_LOG(Warning);
			} else if (Level == LogLevel::Error) {
				UNREALCLR_FRAMEWORK_LOG(Error);
			} else if (Level == LogLevel::Fatal) {
				UNREALCLR_FRAMEWORK_LOG(Fatal);
			}
		}

		void Exception(const char* Message) {
			GEngine->AddOnScreenDebugMessage((uint64)-1, 10.0f, FColor::Red, *FString(UTF8_TO_TCHAR(Message)));
		}

		void AddOnScreenMessage(int32 Key, float TimeToDisplay, Color DisplayColor, const char* Message) {
			GEngine->AddOnScreenDebugMessage((uint64)Key, TimeToDisplay, DisplayColor, *FString(UTF8_TO_TCHAR(Message)));
		}

		void ClearOnScreenMessages() {
			GEngine->ClearOnScreenDebugMessages();
		}
	}

	namespace Application {
		bool IsCanEverRender() {
			return FApp::CanEverRender();
		}

		bool IsPackagedForDistribution() {
			return FGenericPlatformMisc::IsPackagedForDistribution();
		}

		bool IsPackagedForShipping() {
			#if UE_BUILD_SHIPPING
				return true;
			#else
				return false;
			#endif
		}

		void GetProjectDirectory(char* Directory) {
			const char* directory = TCHAR_TO_UTF8(*FPaths::ConvertRelativePathToFull(FPaths::ProjectDir()));

			UnrealCLR::Utility::Strcpy(Directory, directory, UnrealCLR::Utility::Strlen(directory));
		}

		void GetDefaultLanguage(char* Language) {
			const char* language = TCHAR_TO_UTF8(*FGenericPlatformMisc::GetDefaultLanguage());

			UnrealCLR::Utility::Strcpy(Language, language, UnrealCLR::Utility::Strlen(language));
		}

		void GetProjectName(char* ProjectName) {
			const char* projectName = TCHAR_TO_UTF8(FApp::GetProjectName());

			UnrealCLR::Utility::Strcpy(ProjectName, projectName, UnrealCLR::Utility::Strlen(projectName));
		}

		float GetVolumeMultiplier() {
			return FApp::GetVolumeMultiplier();
		}

		void SetProjectName(const char* ProjectName) {
			FApp::SetProjectName(UTF8_TO_TCHAR(ProjectName));
		}

		void SetVolumeMultiplier(float Value) {
			FApp::SetVolumeMultiplier(Value);
		}

		void RequestExit(bool Force) {
			FGenericPlatformMisc::RequestExit(Force);
		}
	}
}

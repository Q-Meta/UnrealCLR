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

#pragma once

UNREALCLR_API DECLARE_LOG_CATEGORY_EXTERN(LogUnrealManaged, Log, All);

namespace UnrealCLRFramework {
	enum struct LogLevel : int32 {
		Display,
		Warning,
		Error,
		Fatal
	};

	struct Color {
		uint8 B;
		uint8 G;
		uint8 R;
		uint8 A;

		FORCEINLINE Color(FColor Value) {
			this->R = Value.R;
			this->G = Value.G;
			this->B = Value.B;
			this->A = Value.A;
		}

		FORCEINLINE operator FColor() const { return FColor(R, G, B, A); }
	};

	// Enumerable
	// 
	// Non-instantiable

	namespace Assert {
		static void OutputMessage(const char* Message);
	}

	namespace CommandLine {
		static void Get(char* Arguments);
		static void Set(const char* Arguments);
		static void Append(const char* Arguments);
	}

	namespace Debug {
		static void Log(LogLevel Level, const char* Message);
		static void Exception(const char* Exception);
		static void AddOnScreenMessage(int32 Key, float TimeToDisplay, Color DisplayColor, const char* Message);
		static void ClearOnScreenMessages();
	}

	namespace Application {
		static bool IsCanEverRender();
		static bool IsPackagedForDistribution();
		static bool IsPackagedForShipping();
		static void GetProjectDirectory(char* Directory);
		static void GetDefaultLanguage(char* Language);
		static void GetProjectName(char* ProjectName);
		static float GetVolumeMultiplier();
		static void SetProjectName(const char* ProjectName);
		static void SetVolumeMultiplier(float Value);
		static void RequestExit(bool Force);
	}

}
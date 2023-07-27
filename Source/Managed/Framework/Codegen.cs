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

using System;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Framework {
	// Automatically generated

	internal static class Shared {
		internal const int checksum = 0x16; //0x2F0;
		internal static Dictionary<int, IntPtr> userFunctions = new();
		private const string dynamicTypesAssemblyName = "UnrealEngine.DynamicTypes";
		private static readonly ModuleBuilder moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(new(dynamicTypesAssemblyName), AssemblyBuilderAccess.RunAndCollect).DefineDynamicModule(dynamicTypesAssemblyName);
		private static readonly Type[] delegateCtorSignature = { typeof(object), typeof(IntPtr) };
		private static Dictionary<string, Delegate> delegatesCache = new();
		private static Dictionary<string, Type> delegateTypesCache = new();
		private const MethodAttributes ctorAttributes = MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;
		private const MethodImplAttributes implAttributes = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
		private const MethodAttributes invokeAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
		private const TypeAttributes delegateTypeAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass;

		internal static unsafe Dictionary<int, IntPtr> Load(IntPtr* events, IntPtr functions, Assembly pluginAssembly) {
			int position = 0;
			IntPtr* buffer = (IntPtr*)functions;

			unchecked {
				int head = 0;
				IntPtr* assertFunctions = (IntPtr*)buffer[position++];

				Assert.outputMessage = (delegate* unmanaged[Cdecl]<byte[], void>)assertFunctions[head++];
			}

			unchecked {
				int head = 0;
				IntPtr* commandLineFunctions = (IntPtr*)buffer[position++];

				CommandLine.get = (delegate* unmanaged[Cdecl]<byte[], void>)commandLineFunctions[head++];
				CommandLine.set = (delegate* unmanaged[Cdecl]<byte[], void>)commandLineFunctions[head++];
				CommandLine.append = (delegate* unmanaged[Cdecl]<byte[], void>)commandLineFunctions[head++];
			}

			unchecked {
				int head = 0;
				IntPtr* debugFunctions = (IntPtr*)buffer[position++];

				Debug.log = (delegate* unmanaged[Cdecl]<LogLevel, byte[], void>)debugFunctions[head++];
				Debug.exception = (delegate* unmanaged[Cdecl]<byte[], void>)debugFunctions[head++];
				Debug.addOnScreenMessage = (delegate* unmanaged[Cdecl]<int, float, int, byte[], void>)debugFunctions[head++];
				Debug.clearOnScreenMessages = (delegate* unmanaged[Cdecl]<void>)debugFunctions[head++];
			}

            unchecked
            {
                int head = 0;
                IntPtr* applicationFunctions = (IntPtr*)buffer[position++];

                Application.isCanEverRender = (delegate* unmanaged[Cdecl]<Bool>)applicationFunctions[head++];
                Application.isPackagedForDistribution = (delegate* unmanaged[Cdecl]<Bool>)applicationFunctions[head++];
                Application.isPackagedForShipping = (delegate* unmanaged[Cdecl]<Bool>)applicationFunctions[head++];
                Application.getProjectDirectory = (delegate* unmanaged[Cdecl]<byte[], void>)applicationFunctions[head++];
                Application.getDefaultLanguage = (delegate* unmanaged[Cdecl]<byte[], void>)applicationFunctions[head++];
                Application.getProjectName = (delegate* unmanaged[Cdecl]<byte[], void>)applicationFunctions[head++];
                Application.getVolumeMultiplier = (delegate* unmanaged[Cdecl]<float>)applicationFunctions[head++];
                Application.setProjectName = (delegate* unmanaged[Cdecl]<byte[], void>)applicationFunctions[head++];
                Application.setVolumeMultiplier = (delegate* unmanaged[Cdecl]<float, void>)applicationFunctions[head++];
                Application.requestExit = (delegate* unmanaged[Cdecl]<Bool, void>)applicationFunctions[head++];
            }

            unchecked {
				Type[] types = pluginAssembly.GetTypes();

				foreach (Type type in types) {
					MethodInfo[] methods = type.GetMethods();

					if (type.Name == "Main" && type.IsPublic) {
						foreach (MethodInfo method in methods) {
							if (method.IsPublic && method.IsStatic && !method.IsGenericMethod) {
								ParameterInfo[] parameterInfos = method.GetParameters();

								if (parameterInfos.Length <= 1) {
									if (method.Name == "OnWorldBegin") {
										if (parameterInfos.Length == 0)
											events[0] = GetFunctionPointer(method);
										else
											throw new ArgumentException(method.Name + " should not have arguments");

										continue;
									}

									if (method.Name == "OnWorldPostBegin") {
										if (parameterInfos.Length == 0)
											events[1] = GetFunctionPointer(method);
										else
											throw new ArgumentException(method.Name + " should not have arguments");

										continue;
									}

									if (method.Name == "OnWorldPrePhysicsTick") {
										if (parameterInfos.Length == 1 && parameterInfos[0].ParameterType == typeof(float))
											events[2] = GetFunctionPointer(method);
										else
											throw new ArgumentException(method.Name + " should have a float argument");

										continue;
									}

									if (method.Name == "OnWorldDuringPhysicsTick") {
										if (parameterInfos.Length == 1 && parameterInfos[0].ParameterType == typeof(float))
											events[3] = GetFunctionPointer(method);
										else
											throw new ArgumentException(method.Name + " should have a float argument");

										continue;
									}

									if (method.Name == "OnWorldPostPhysicsTick") {
										if (parameterInfos.Length == 1 && parameterInfos[0].ParameterType == typeof(float))
											events[4] = GetFunctionPointer(method);
										else
											throw new ArgumentException(method.Name + " should have a float argument");

										continue;
									}

									if (method.Name == "OnWorldPostUpdateTick") {
										if (parameterInfos.Length == 1 && parameterInfos[0].ParameterType == typeof(float))
											events[5] = GetFunctionPointer(method);
										else
											throw new ArgumentException(method.Name + " should have a float argument");

										continue;
									}

									if (method.Name == "OnWorldEnd") {
										if (parameterInfos.Length == 0)
											events[6] = GetFunctionPointer(method);
										else
											throw new ArgumentException(method.Name + " should not have arguments");

										continue;
									}
								}
							}
						}
					}

					foreach (MethodInfo method in methods) {
						if (method.IsPublic && method.IsStatic && !method.IsGenericMethod) {
							ParameterInfo[] parameterInfos = method.GetParameters();

							if (parameterInfos.Length <= 1) {
								if (parameterInfos.Length == 1 && parameterInfos[0].ParameterType != typeof(ObjectReference))
									continue;

								string name = type.FullName + "." + method.Name;

								userFunctions.Add(name.GetHashCode(StringComparison.Ordinal), GetFunctionPointer(method));
							}
						}
					}
				}
			}

            Directory.SetCurrentDirectory(Application.ProjectDirectory);

			GC.Collect();
			GC.WaitForPendingFinalizers();

			return userFunctions;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string GetTypeName(Type type) => type.FullName.Replace(".", string.Empty, StringComparison.Ordinal);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string GetMethodName(Type[] parameters, Type returnType) {
			string name = GetTypeName(returnType);

			foreach (Type type in parameters) {
				name += '_' + GetTypeName(type);
			}

			return name;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Type GetDelegateType(Type[] parameters, Type returnType) {
			string methodName = GetMethodName(parameters, returnType);

			return delegateTypesCache.GetOrAdd(methodName, () => MakeDelegate(parameters, returnType, methodName));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Type MakeDelegate(Type[] types, Type returnType, string name) {
			TypeBuilder builder = moduleBuilder.DefineType(name, delegateTypeAttributes, typeof(MulticastDelegate));

			builder.DefineConstructor(ctorAttributes, CallingConventions.Standard, delegateCtorSignature).SetImplementationFlags(implAttributes);
			builder.DefineMethod("Invoke", invokeAttributes, returnType, types).SetImplementationFlags(implAttributes);

			return builder.CreateTypeInfo();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IntPtr GetFunctionPointer(MethodInfo method) {
			string methodName = $"{ method.DeclaringType.FullName }.{ method.Name }";

			Delegate dynamicDelegate = delegatesCache.GetOrAdd(methodName, () => {
				ParameterInfo[] parameterInfos = method.GetParameters();
				Type[] parameterTypes = new Type[parameterInfos.Length];

				for (int i = 0; i < parameterTypes.Length; i++) {
					parameterTypes[i] = parameterInfos[i].ParameterType;
				}

				return method.CreateDelegate(GetDelegateType(parameterTypes, method.ReturnType));
			});

			return Collector.GetFunctionPointer(dynamicDelegate);
		}
	}

	internal struct Bool {
		private byte value;

		public Bool(byte value) => this.value = value;

		public static implicit operator bool(Bool value) => value.value != 0;

		public static implicit operator Bool(bool value) => !value ? new(0) : new(1);

		public override int GetHashCode() => value.GetHashCode();
	}

	static unsafe partial class Assert {
		internal static delegate* unmanaged[Cdecl]<byte[], void> outputMessage;
	}

	static unsafe partial class CommandLine {
		internal static delegate* unmanaged[Cdecl]<byte[], void> get;
		internal static delegate* unmanaged[Cdecl]<byte[], void> set;
		internal static delegate* unmanaged[Cdecl]<byte[], void> append;
	}

	static unsafe partial class Debug {
		internal static delegate* unmanaged[Cdecl]<LogLevel, byte[], void> log;
		internal static delegate* unmanaged[Cdecl]<byte[], void> exception;
		internal static delegate* unmanaged[Cdecl]<int, float, int, byte[], void> addOnScreenMessage;
		internal static delegate* unmanaged[Cdecl]<void> clearOnScreenMessages;
	}

	static unsafe partial class Application {
		internal static delegate* unmanaged[Cdecl]<Bool> isCanEverRender;
		internal static delegate* unmanaged[Cdecl]<Bool> isPackagedForDistribution;
		internal static delegate* unmanaged[Cdecl]<Bool> isPackagedForShipping;
		internal static delegate* unmanaged[Cdecl]<byte[], void> getProjectDirectory;
		internal static delegate* unmanaged[Cdecl]<byte[], void> getDefaultLanguage;
		internal static delegate* unmanaged[Cdecl]<byte[], void> getProjectName;
		internal static delegate* unmanaged[Cdecl]<float> getVolumeMultiplier;
		internal static delegate* unmanaged[Cdecl]<byte[], void> setProjectName;
		internal static delegate* unmanaged[Cdecl]<float, void> setVolumeMultiplier;
		internal static delegate* unmanaged[Cdecl]<Bool, void> requestExit;
	}
    internal static unsafe class Object
    {
        internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isValid;
        internal static delegate* unmanaged[Cdecl]<IntPtr, uint> getID;
        internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> getName;
    }
}

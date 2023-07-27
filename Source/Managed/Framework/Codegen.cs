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

	[StructLayout(LayoutKind.Sequential)]
	partial struct LinearColor {
		private float r;
		private float g;
		private float b;
		private float a;
	}

	[StructLayout(LayoutKind.Sequential)]
	partial struct Transform {
		private Vector3 location;
		private Quaternion rotation;
		private Vector3 scale;
	}

	[StructLayout(LayoutKind.Sequential)]
	partial struct Hit {
		private Vector3 location;
		private Vector3 impactLocation;
		private Vector3 normal;
		private Vector3 impactNormal;
		private Vector3 traceStart;
		private Vector3 traceEnd;
		private IntPtr actor;
		private float time;
		private float distance;
		private float penetrationDepth;
		private Bool blockingHit;
		private Bool startPenetrating;
	}

	[StructLayout(LayoutKind.Explicit, Size = 28)]
	partial struct Bounds {
		[FieldOffset(0)]
		private Vector3 origin;
		[FieldOffset(12)]
		private Vector3 boxExtent;
		[FieldOffset(24)]
		private float sphereRadius;
	}

	[StructLayout(LayoutKind.Explicit, Size = 16)]
	partial struct CollisionShape {
		[FieldOffset(0)]
		private CollisionShapeType shapeType;
		[FieldOffset(4)]
		private Box box;
		[FieldOffset(4)]
		private Sphere sphere;
		[FieldOffset(4)]
		private Capsule capsule;

		private struct Box {
			internal Vector3 halfExtent;
		}

		private struct Sphere {
			internal float radius;
		}

		private struct Capsule {
			internal float radius;
			internal float halfHeight;
		}
	}

	internal struct Bool {
		private byte value;

		public Bool(byte value) => this.value = value;

		public static implicit operator bool(Bool value) => value.value != 0;

		public static implicit operator Bool(bool value) => !value ? new(0) : new(1);

		public override int GetHashCode() => value.GetHashCode();
	}

	internal enum ObjectType : int {
		Blueprint,
		SoundWave,
		AnimationSequence,
		AnimationMontage,
		StaticMesh,
		SkeletalMesh,
		Material,
		Font,
		Texture2D
	}

	internal enum ActorType : int {
		Base,
		Camera,
		TriggerBox,
		TriggerSphere,
		TriggerCapsule,
		Pawn,
		Character,
		AIController,
		PlayerController,
		Brush,
		AmbientSound,
		DirectionalLight,
		PointLight,
		RectLight,
		SpotLight,
		TriggerVolume,
		PostProcessVolume,
		LevelScript,
		GameModeBase
	}

	internal enum ComponentType : int {
		// Non-movable
		Actor,
		Input,
		Movement,
		RotatingMovement,
		// Movable
		Scene,
		Audio,
		Camera,
		Light,
		DirectionalLight,
		MotionController,
		StaticMesh,
		InstancedStaticMesh,
		HierarchicalInstancedStaticMesh,
		ChildActor,
		SpringArm,
		PostProcess,
		Box,
		Sphere,
		Capsule,
		TextRender,
		SkeletalMesh,
		Spline,
		RadialForce
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
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, in Quaternion, int, Bool, float, byte, float, void> drawBox;
		internal static delegate* unmanaged[Cdecl]<in Vector3, float, float, in Quaternion, int, Bool, float, byte, float, void> drawCapsule;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, float, float, float, int, int, Bool, float, byte, float, void> drawCone;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, float, int, int, Bool, float, byte, float, void> drawCylinder;
		internal static delegate* unmanaged[Cdecl]<in Vector3, float, int, int, Bool, float, byte, float, void> drawSphere;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, int, Bool, float, byte, float, void> drawLine;
		internal static delegate* unmanaged[Cdecl]<in Vector3, float, int, Bool, float, byte, void> drawPoint;
		internal static delegate* unmanaged[Cdecl]<void> flushPersistentLines;
	}

	internal static unsafe class Object {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isPendingKill;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isValid;
		internal static delegate* unmanaged[Cdecl]<ObjectType, byte[], IntPtr> load;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> rename;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool> invoke;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ActorType, IntPtr> toActor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ComponentType, IntPtr> toComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, uint> getID;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> getName;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref bool, Bool> getBool;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref byte, Bool> getByte;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref short, Bool> getShort;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref int, Bool> getInt;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref long, Bool> getLong;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref ushort, Bool> getUShort;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref uint, Bool> getUInt;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref ulong, Bool> getULong;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref float, Bool> getFloat;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref double, Bool> getDouble;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref int, Bool> getEnum;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], byte[], Bool> getString;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], byte[], Bool> getText;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool, Bool> setBool;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], byte, Bool> setByte;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], short, Bool> setShort;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], int, Bool> setInt;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], long, Bool> setLong;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ushort, Bool> setUShort;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], uint, Bool> setUInt;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ulong, Bool> setULong;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], float, Bool> setFloat;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], double, Bool> setDouble;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], int, Bool> setEnum;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], byte[], Bool> setString;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], byte[], Bool> setText;
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

	static unsafe partial class ConsoleManager {
		internal static delegate* unmanaged[Cdecl]<byte[], Bool> isRegisteredVariable;
		internal static delegate* unmanaged[Cdecl]<byte[], IntPtr> findVariable;
		internal static delegate* unmanaged[Cdecl]<byte[], byte[], Bool, Bool, IntPtr> registerVariableBool;
		internal static delegate* unmanaged[Cdecl]<byte[], byte[], int, Bool, IntPtr> registerVariableInt;
		internal static delegate* unmanaged[Cdecl]<byte[], byte[], float, Bool, IntPtr> registerVariableFloat;
		internal static delegate* unmanaged[Cdecl]<byte[], byte[], byte[], Bool, IntPtr> registerVariableString;
		internal static delegate* unmanaged[Cdecl]<byte[], byte[], IntPtr, Bool, void> registerCommand;
		internal static delegate* unmanaged[Cdecl]<byte[], void> unregisterObject;
	}

	static unsafe partial class Engine {
		internal static delegate* unmanaged[Cdecl]<Bool> isSplitScreen;
		internal static delegate* unmanaged[Cdecl]<Bool> isEditor;
		internal static delegate* unmanaged[Cdecl]<Bool> isForegroundWindow;
		internal static delegate* unmanaged[Cdecl]<Bool> isExitRequested;
		internal static delegate* unmanaged[Cdecl]<NetMode> getNetMode;
		internal static delegate* unmanaged[Cdecl]<uint> getFrameNumber;
		internal static delegate* unmanaged[Cdecl]<ref Vector2, void> getViewportSize;
		internal static delegate* unmanaged[Cdecl]<ref Vector2, void> getScreenResolution;
		internal static delegate* unmanaged[Cdecl]<WindowMode> getWindowMode;
		internal static delegate* unmanaged[Cdecl]<byte[], void> getVersion;
		internal static delegate* unmanaged[Cdecl]<float> getMaxFPS;
		internal static delegate* unmanaged[Cdecl]<float, void> setMaxFPS;
		internal static delegate* unmanaged[Cdecl]<byte[], void> setTitle;
		internal static delegate* unmanaged[Cdecl]<byte[], byte[], Bool, Bool, Bool, Bool, void> addActionMapping;
		internal static delegate* unmanaged[Cdecl]<byte[], byte[], float, void> addAxisMapping;
		internal static delegate* unmanaged[Cdecl]<Bool, void> forceGarbageCollection;
		internal static delegate* unmanaged[Cdecl]<void> delayGarbageCollection;
	}

	static unsafe partial class HeadMountedDisplay {
		internal static delegate* unmanaged[Cdecl]<Bool> isConnected;
		internal static delegate* unmanaged[Cdecl]<Bool> getEnabled;
		internal static delegate* unmanaged[Cdecl]<Bool> getLowPersistenceMode;
		internal static delegate* unmanaged[Cdecl]<byte[], void> getDeviceName;
		internal static delegate* unmanaged[Cdecl]<Bool, void> setEnable;
		internal static delegate* unmanaged[Cdecl]<Bool, void> setLowPersistenceMode;
	}

	static unsafe partial class World {
		internal static delegate* unmanaged[Cdecl]<ref ObjectReference*, ref int, void> forEachActor;
		internal static delegate* unmanaged[Cdecl]<int> getActorCount;
		internal static delegate* unmanaged[Cdecl]<float> getDeltaSeconds;
		internal static delegate* unmanaged[Cdecl]<float> getRealTimeSeconds;
		internal static delegate* unmanaged[Cdecl]<float> getTimeSeconds;
		internal static delegate* unmanaged[Cdecl]<byte[], void> getCurrentLevelName;
		internal static delegate* unmanaged[Cdecl]<Bool> getSimulatePhysics;
		internal static delegate* unmanaged[Cdecl]<ref Vector3, void> getWorldOrigin;
		internal static delegate* unmanaged[Cdecl]<byte[], ActorType, IntPtr> getActor;
		internal static delegate* unmanaged[Cdecl]<byte[], ActorType, IntPtr> getActorByTag;
		internal static delegate* unmanaged[Cdecl]<uint, ActorType, IntPtr> getActorByID;
		internal static delegate* unmanaged[Cdecl]<IntPtr> getFirstPlayerController;
		internal static delegate* unmanaged[Cdecl]<IntPtr> getGameMode;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnActorBeginOverlapCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnActorEndOverlapCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnActorHitCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnActorBeginCursorOverCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnActorEndCursorOverCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnActorClickedCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnActorReleasedCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnComponentBeginOverlapCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnComponentEndOverlapCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnComponentHitCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnComponentBeginCursorOverCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnComponentEndCursorOverCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnComponentClickedCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> setOnComponentReleasedCallback;
		internal static delegate* unmanaged[Cdecl]<Bool, void> setSimulatePhysics;
		internal static delegate* unmanaged[Cdecl]<float, void> setGravity;
		internal static delegate* unmanaged[Cdecl]<in Vector3, Bool> setWorldOrigin;
		internal static delegate* unmanaged[Cdecl]<byte[], void> openLevel;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, CollisionChannel, Bool, IntPtr, IntPtr, Bool> lineTraceTestByChannel;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, byte[], Bool, IntPtr, IntPtr, Bool> lineTraceTestByProfile;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, CollisionChannel, ref Hit, byte[], Bool, IntPtr, IntPtr, Bool> lineTraceSingleByChannel;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, byte[], ref Hit, byte[], Bool, IntPtr, IntPtr, Bool> lineTraceSingleByProfile;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, in Quaternion, CollisionChannel, in CollisionShape, Bool, IntPtr, IntPtr, Bool> sweepTestByChannel;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, in Quaternion, byte[], in CollisionShape, Bool, IntPtr, IntPtr, Bool> sweepTestByProfile;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, in Quaternion, CollisionChannel, in CollisionShape, ref Hit, byte[], Bool, IntPtr, IntPtr, Bool> sweepSingleByChannel;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Vector3, in Quaternion, byte[], in CollisionShape, ref Hit, byte[], Bool, IntPtr, IntPtr, Bool> sweepSingleByProfile;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Quaternion, CollisionChannel, in CollisionShape, IntPtr, IntPtr, Bool> overlapAnyTestByChannel;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Quaternion, byte[], in CollisionShape, IntPtr, IntPtr, Bool> overlapAnyTestByProfile;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Quaternion, CollisionChannel, in CollisionShape, IntPtr, IntPtr, Bool> overlapBlockingTestByChannel;
		internal static delegate* unmanaged[Cdecl]<in Vector3, in Quaternion, byte[], in CollisionShape, IntPtr, IntPtr, Bool> overlapBlockingTestByProfile;
	}

	unsafe partial struct Asset {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isValid;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> getName;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> getPath;
	}

	unsafe partial class AssetRegistry {
		internal static delegate* unmanaged[Cdecl]<IntPtr> get;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool, Bool> hasAssets;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool, Bool, ref Asset*, ref int, void> forEachAsset;
	}

	unsafe partial class Blueprint {
		internal static delegate* unmanaged[Cdecl]<IntPtr, ActorType, Bool> isValidActorClass;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ComponentType, Bool> isValidComponentClass;
	}

	unsafe partial class ConsoleObject {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isBool;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isInt;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isFloat;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isString;
	}

	unsafe partial class ConsoleVariable {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getBool;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int> getInt;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getFloat;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> getString;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setBool;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> setInt;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setFloat;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> setString;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setOnChangedCallback;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> clearOnChangedCallback;
	}

	unsafe partial class Actor {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isPendingKill;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isRootComponentMovable;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool> isOverlappingActor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref ObjectReference*, ref int, void> forEachComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref ObjectReference*, ref int, void> forEachAttachedActor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref ObjectReference*, ref int, void> forEachChildActor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref ObjectReference*, ref int, void> forEachOverlappingActor;
		internal static delegate* unmanaged[Cdecl]<byte[], ActorType, IntPtr, IntPtr> spawn;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> destroy;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> rename;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> hide;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, in Quaternion, Bool, Bool, Bool> teleportTo;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ComponentType, IntPtr> getComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ComponentType, IntPtr> getComponentByTag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, uint, ComponentType, IntPtr> getComponentByID;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ComponentType, IntPtr> getRootComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getInputComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getCreationTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getBlockInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float> getDistanceTo;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float> getHorizontalDistanceTo;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, ref Vector3, ref Vector3, void> getBounds;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, ref Quaternion, void> getEyesViewPoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool> setRootComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setInputComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setBlockInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setLifeSpan;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool, void> setEnableInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setEnableCollision;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> addTag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> removeTag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool> hasTag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ActorEventType, void> registerEvent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ActorEventType, void> unregisterEvent;
	}

	unsafe partial class GameModeBase {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getUseSeamlessTravel;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setUseSeamlessTravel;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, void> swapPlayerControllers;
	}

	unsafe partial class TriggerBase { }

	unsafe partial class TriggerBox { }

	unsafe partial class TriggerCapsule { }

	unsafe partial class TriggerSphere { }

	unsafe partial class Pawn {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isControlled;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isPlayerControlled;
		internal static delegate* unmanaged[Cdecl]<IntPtr, AutoPossessAI> getAutoPossessAI;
		internal static delegate* unmanaged[Cdecl]<IntPtr, AutoReceiveInput> getAutoPossessPlayer;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getUseControllerRotationYaw;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getUseControllerRotationPitch;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getUseControllerRotationRoll;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getGravityDirection;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getAIController;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getPlayerController;
		internal static delegate* unmanaged[Cdecl]<IntPtr, AutoPossessAI, void> setAutoPossessAI;
		internal static delegate* unmanaged[Cdecl]<IntPtr, AutoReceiveInput, void> setAutoPossessPlayer;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setUseControllerRotationYaw;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setUseControllerRotationPitch;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setUseControllerRotationRoll;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> addControllerYawInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> addControllerPitchInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> addControllerRollInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, float, Bool, void> addMovementInput;
	}

	unsafe partial class Character {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isCrouched;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> canCrouch;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> canJump;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> checkJumpInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> clearJumpInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, Bool, Bool, void> launch;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> crouch;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> stopCrouching;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> jump;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> stopJumping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setOnLandedCallback;
	}

	unsafe partial class Controller {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isLookInputIgnored;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isMoveInputIgnored;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isPlayerController;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getPawn;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getCharacter;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getViewTarget;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Quaternion, void> getControlRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Quaternion, void> getDesiredRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, in Vector3, Bool, Bool> lineOfSightTo;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Quaternion, void> setControlRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, in Quaternion, void> setInitialLocationAndRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setIgnoreLookInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setIgnoreMoveInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> resetIgnoreLookInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> resetIgnoreMoveInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> possess;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> unpossess;
	}

	unsafe partial class AIController {
		internal static delegate* unmanaged[Cdecl]<IntPtr, AIFocusPriority, void> clearFocus;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getFocalPoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, AIFocusPriority, void> setFocalPoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getFocusActor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getAllowStrafe;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setAllowStrafe;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, AIFocusPriority, void> setFocus;
	}

	unsafe partial class PlayerController {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isPaused;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getShowMouseCursor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getEnableClickEvents;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getEnableMouseOverEvents;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref float, ref float, Bool> getMousePosition;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getPlayer;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getPlayerInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector2, CollisionChannel, ref Hit, Bool, Bool> getHitResultAtScreenPosition;
		internal static delegate* unmanaged[Cdecl]<IntPtr, CollisionChannel, ref Hit, Bool, Bool> getHitResultUnderCursor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setShowMouseCursor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setEnableClickEvents;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setEnableMouseOverEvents;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, float, void> setMousePosition;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool, void> consoleCommand;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, Bool> setPause;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setViewTarget;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float, float, BlendType, Bool, void> setViewTargetWithBlend;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> addYawInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> addPitchInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> addRollInput;
	}

	unsafe partial class Volume {
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, float, ref float, Bool> encompassesPoint;
	}

	unsafe partial class TriggerVolume { }

	unsafe partial class PostProcessVolume {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getEnabled;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getBlendRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getBlendWeight;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getUnbound;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getPriority;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setEnabled;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setBlendRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setBlendWeight;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setUnbound;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setPriority;
	}

	unsafe partial class LevelScript { }

	unsafe partial class AmbientSound { }

	unsafe partial class Light { }

	unsafe partial class DirectionalLight { }

	unsafe partial class PointLight { }

	unsafe partial class RectLight { }

	unsafe partial class SpotLight { }

	unsafe partial class SoundBase {
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getDuration;
	}

	unsafe partial class SoundWave {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getLoop;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setLoop;
	}

	unsafe partial class AnimationAsset { }

	unsafe partial class AnimationSequenceBase { }

	unsafe partial class AnimationSequence { }

	unsafe partial class AnimationCompositeBase { }

	unsafe partial class AnimationMontage { }

	unsafe partial class AnimationInstance {
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getCurrentActiveMontage;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool> isPlaying;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float> getPlayRate;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float> getPosition;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float> getBlendTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, byte[], void> getCurrentSection;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float, void> setPlayRate;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float, void> setPosition;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, byte[], byte[], void> setNextSection;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float, float, Bool, float> playMontage;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> pauseMontage;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> resumeMontage;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, float, void> stopMontage;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, byte[], void> jumpToSection;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, byte[], void> jumpToSectionsEnd;
	}

	unsafe partial class Player {
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getPlayerController;
	}

	unsafe partial class PlayerInput {
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool> isKeyPressed;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], float> getTimeKeyPressed;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector2, void> getMouseSensitivity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector2, void> setMouseSensitivity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], byte[], Bool, Bool, Bool, Bool, void> addActionMapping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], byte[], float, void> addAxisMapping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], byte[], void> removeActionMapping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], byte[], void> removeAxisMapping;
	}

	unsafe partial class Font {
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref int, ref int, void> getStringSize;
	}

	unsafe partial class StreamableRenderAsset { }

	unsafe partial class StaticMesh { }

	unsafe partial class SkeletalMesh { }

	unsafe partial class Texture { }

	unsafe partial class Texture2D {
		internal static delegate* unmanaged[Cdecl]<byte[], IntPtr> createFromFile;
		internal static delegate* unmanaged[Cdecl]<byte[], int, IntPtr> createFromBuffer;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> hasAlphaChannel;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector2, void> getSize;
		internal static delegate* unmanaged[Cdecl]<IntPtr, PixelFormat> getPixelFormat;
	}

	unsafe partial class ActorComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isOwnerSelected;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ActorType, IntPtr> getOwner;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> destroy;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> addTag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> removeTag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool> hasTag;
	}

	unsafe partial class InputComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> hasBindings;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int> getActionBindingsNumber;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> clearActionBindings;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], InputEvent, Bool, IntPtr, void> bindAction;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool, IntPtr, void> bindAxis;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], InputEvent, void> removeActionBinding;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getBlockInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setBlockInput;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int> getPriority;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> setPriority;
	}

	unsafe partial class MovementComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getConstrainToPlane;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getSnapToPlaneAtStart;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getUpdateOnlyIfRendered;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getVelocity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, PlaneConstraintAxis> getPlaneConstraint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getPlaneConstraintNormal;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getPlaneConstraintOrigin;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getGravity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getMaxSpeed;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setConstrainToPlane;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setSnapToPlaneAtStart;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setUpdateOnlyIfRendered;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> setVelocity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, PlaneConstraintAxis, void> setPlaneConstraint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> setPlaneConstraintNormal;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> setPlaneConstraintOrigin;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, in Vector3, void> setPlaneConstraintFromVectors;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, Bool> isExceedingMaxSpeed;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isInWater;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> stopMovement;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, ref Vector3, void> constrainDirectionToPlane;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, ref Vector3, void> constrainLocationToPlane;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, ref Vector3, void> constrainNormalToPlane;
	}

	unsafe partial class RotatingMovementComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], IntPtr> create;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getRotationInLocalSpace;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getPivotTranslation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Quaternion, void> getRotationRate;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setRotationInLocalSpace;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> setPivotTranslation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Quaternion, void> setRotationRate;
	}

	unsafe partial class SceneComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool> isAttachedToComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool> isAttachedToActor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isVisible;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool> isSocketExists;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> hasAnySockets;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, byte[], Bool> canAttachAsChild;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref ObjectReference*, ref int, void> forEachAttachedChild;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ComponentType, byte[], Bool, IntPtr, IntPtr> create;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, AttachmentTransformRule, byte[], Bool> attachToComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, DetachmentTransformRule, void> detachFromComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> activate;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> deactivate;
		internal static delegate* unmanaged[Cdecl]<IntPtr, TeleportType, UpdateTransformFlags, void> updateToWorld;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> addLocalOffset;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Quaternion, void> addLocalRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> addRelativeLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Quaternion, void> addRelativeRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Transform, void> addLocalTransform;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> addWorldOffset;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Quaternion, void> addWorldRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Transform, void> addWorldTransform;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> getAttachedSocketName;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Transform, ref Bounds, void> getBounds;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref Vector3, void> getSocketLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], ref Quaternion, void> getSocketRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getComponentVelocity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getComponentLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Quaternion, void> getComponentRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getComponentScale;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Transform, void> getComponentTransform;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getForwardVector;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getRightVector;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getUpVector;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ComponentMobility, void> setMobility;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, Bool, void> setVisibility;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> setRelativeLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Quaternion, void> setRelativeRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Transform, void> setRelativeTransform;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> setWorldLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Quaternion, void> setWorldRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> setWorldScale;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Transform, void> setWorldTransform;
	}

	unsafe partial class AudioComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isPlaying;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getPaused;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setSound;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setPaused;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> play;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> stop;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, float, float, AudioFadeCurve, void> fadeIn;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, float, AudioFadeCurve, void> fadeOut;
	}

	unsafe partial class CameraComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getConstrainAspectRatio;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getAspectRatio;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getFieldOfView;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getOrthoFarClipPlane;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getOrthoNearClipPlane;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getOrthoWidth;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getLockToHeadMountedDisplay;
		internal static delegate* unmanaged[Cdecl]<IntPtr, CameraProjectionMode, void> setProjectionMode;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setConstrainAspectRatio;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setAspectRatio;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setFieldOfView;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setOrthoFarClipPlane;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setOrthoNearClipPlane;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setOrthoWidth;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setLockToHeadMountedDisplay;
	}

	unsafe partial class ChildActorComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, ActorType, IntPtr> getChildActor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ActorType, IntPtr> setChildActor;
	}

	unsafe partial class SpringArmComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isCollisionFixApplied;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getDrawDebugLagMarkers;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getCollisionTest;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getCameraPositionLag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getCameraRotationLag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getCameraLagSubstepping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getInheritPitch;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getInheritRoll;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getInheritYaw;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getCameraLagMaxDistance;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getCameraLagMaxTimeStep;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getCameraPositionLagSpeed;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getCameraRotationLagSpeed;
		internal static delegate* unmanaged[Cdecl]<IntPtr, CollisionChannel> getProbeChannel;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getProbeSize;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getSocketOffset;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getTargetArmLength;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getTargetOffset;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getUnfixedCameraPosition;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Quaternion, void> getDesiredRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Quaternion, void> getTargetRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getUsePawnControlRotation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setDrawDebugLagMarkers;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setCollisionTest;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setCameraPositionLag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setCameraRotationLag;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setCameraLagSubstepping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setInheritPitch;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setInheritRoll;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setInheritYaw;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setCameraLagMaxDistance;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setCameraLagMaxTimeStep;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setCameraPositionLagSpeed;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setCameraRotationLagSpeed;
		internal static delegate* unmanaged[Cdecl]<IntPtr, CollisionChannel, void> setProbeChannel;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setProbeSize;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> setSocketOffset;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setTargetArmLength;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> setTargetOffset;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setUsePawnControlRotation;
	}

	unsafe partial class PostProcessComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getEnabled;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getBlendRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getBlendWeight;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getUnbound;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getPriority;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setEnabled;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setBlendRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setBlendWeight;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setUnbound;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setPriority;
	}

	unsafe partial class PrimitiveComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isGravityEnabled;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool> isOverlappingComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref ObjectReference*, ref int, void> forEachOverlappingComponent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, byte[], Bool, void> addAngularImpulseInDegrees;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, byte[], Bool, void> addAngularImpulseInRadians;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, byte[], Bool, void> addForce;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, in Vector3, byte[], Bool, void> addForceAtLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, byte[], Bool, void> addImpulse;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, in Vector3, byte[], void> addImpulseAtLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, float, float, Bool, Bool, void> addRadialForce;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, float, float, Bool, Bool, void> addRadialImpulse;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, byte[], Bool, void> addTorqueInDegrees;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, byte[], Bool, void> addTorqueInRadians;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getMass;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, byte[], void> getPhysicsLinearVelocity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, in Vector3, byte[], void> getPhysicsLinearVelocityAtPoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, byte[], void> getPhysicsAngularVelocityInDegrees;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, byte[], void> getPhysicsAngularVelocityInRadians;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getCastShadow;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getOnlyOwnerSee;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getOwnerNoSee;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getIgnoreRadialForce;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getIgnoreRadialImpulse;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr> getMaterial;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int> getMaterialsNumber;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, ref Vector3, float> getDistanceToCollision;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, ref float, ref Vector3, Bool> getSquaredDistanceToCollision;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getAngularDamping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getLinearDamping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setGenerateOverlapEvents;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setGenerateHitEvents;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, byte[], void> setMass;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, byte[], void> setCenterOfMass;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, Bool, byte[], void> setPhysicsLinearVelocity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, Bool, byte[], void> setPhysicsAngularVelocityInDegrees;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, Bool, byte[], void> setPhysicsAngularVelocityInRadians;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, Bool, byte[], void> setPhysicsMaxAngularVelocityInDegrees;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, Bool, byte[], void> setPhysicsMaxAngularVelocityInRadians;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setCastShadow;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setOnlyOwnerSee;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setOwnerNoSee;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setIgnoreRadialForce;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setIgnoreRadialImpulse;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, void> setMaterial;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setSimulatePhysics;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setAngularDamping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setLinearDamping;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setEnableGravity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, CollisionMode, void> setCollisionMode;
		internal static delegate* unmanaged[Cdecl]<IntPtr, CollisionChannel, void> setCollisionChannel;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool, void> setCollisionProfileName;
		internal static delegate* unmanaged[Cdecl]<IntPtr, CollisionChannel, CollisionResponse, void> setCollisionResponseToChannel;
		internal static delegate* unmanaged[Cdecl]<IntPtr, CollisionResponse, void> setCollisionResponseToAllChannels;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool, void> setIgnoreActorWhenMoving;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool, void> setIgnoreComponentWhenMoving;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> clearMoveIgnoreActors;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> clearMoveIgnoreComponents;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr> createAndSetMaterialInstanceDynamic;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ComponentEventType, void> registerEvent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ComponentEventType, void> unregisterEvent;
	}

	unsafe partial class ShapeComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getDynamicObstacle;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int> getShapeColor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setDynamicObstacle;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> setShapeColor;
	}

	unsafe partial class BoxComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getScaledBoxExtent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, void> getUnscaledBoxExtent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, Bool, void> setBoxExtent;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, void> initBoxExtent;
	}

	unsafe partial class SphereComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getScaledSphereRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getUnscaledSphereRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getShapeScale;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, Bool, void> setSphereRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> initSphereRadius;
	}

	unsafe partial class CapsuleComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getScaledCapsuleRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getUnscaledCapsuleRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getShapeScale;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref float, ref float, void> getScaledCapsuleSize;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref float, ref float, void> getUnscaledCapsuleSize;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, Bool, void> setCapsuleRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, float, Bool, void> setCapsuleSize;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, float, void> initCapsuleSize;
	}

	unsafe partial class MeshComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], Bool> isValidMaterialSlotName;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], int> getMaterialIndex;
	}

	unsafe partial class TextRenderComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setFont;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> setText;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setTextMaterial;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> setTextRenderColor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, HorizontalTextAligment, void> setHorizontalAlignment;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setHorizontalSpacingAdjustment;
		internal static delegate* unmanaged[Cdecl]<IntPtr, VerticalTextAligment, void> setVerticalAlignment;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setVerticalSpacingAdjustment;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector2, void> setScale;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setWorldSize;
	}

	unsafe partial class LightComponentBase {
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getIntensity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getCastShadows;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setCastShadows;
	}

	unsafe partial class LightComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setIntensity;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in LinearColor, void> setLightColor;
	}

	unsafe partial class DirectionalLightComponent { }

	unsafe partial class MotionControllerComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isTracked;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getDisplayDeviceModel;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getDisableLowLatencyUpdate;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ControllerHand> getTrackingSource;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setDisplayDeviceModel;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setDisableLowLatencyUpdate;
		internal static delegate* unmanaged[Cdecl]<IntPtr, ControllerHand, void> setTrackingSource;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> setTrackingMotionSource;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> setAssociatedPlayerIndex;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setCustomDisplayMesh;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], void> setDisplayModelSource;
	}

	unsafe partial class StaticMeshComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, ref Vector3, ref Vector3, void> getLocalBounds;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getStaticMesh;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool> setStaticMesh;
	}

	unsafe partial class InstancedStaticMeshComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, int> getInstanceCount;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, ref Transform, Bool, Bool> getInstanceTransform;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Transform, void> addInstance;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, Transform[], void> addInstances;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, in Transform, Bool, Bool, Bool, Bool> updateInstanceTransform;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, int, Transform[], Bool, Bool, Bool, Bool> batchUpdateInstanceTransforms;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, Bool> removeInstance;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> clearInstances;
	}

	unsafe partial class HierarchicalInstancedStaticMeshComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getDisableCollision;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setDisableCollision;
	}

	unsafe partial class SkinnedMeshComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, int> getBonesNumber;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], int> getBoneIndex;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, byte[], void> getBoneName;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, ref Transform, void> getBoneTransform;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool, void> setSkeletalMesh;
	}

	unsafe partial class SkeletalMeshComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isPlaying;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getAnimationInstance;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setAnimation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, AnimationMode, void> setAnimationMode;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> setAnimationBlueprint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> play;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool, void> playAnimation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> stop;
	}

	unsafe partial class SplineComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isClosedLoop;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getDuration;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplinePointType> getSplinePointType;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int> getSplinePointsNumber;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int> getSplineSegmentsNumber;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, ref Vector3, void> getTangentAtDistanceAlongSpline;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, ref Vector3, void> getTangentAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, Bool, ref Vector3, void> getTangentAtTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, ref Transform, void> getTransformAtDistanceAlongSpline;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, Bool, ref Transform, void> getTransformAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, ref Vector3, void> getArriveTangentAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, SplineCoordinateSpace, ref Vector3, void> getDefaultUpVector;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, ref Vector3, void> getDirectionAtDistanceAlongSpline;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, ref Vector3, void> getDirectionAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, Bool, ref Vector3, void> getDirectionAtTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, float> getDistanceAlongSplineAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, ref Vector3, void> getLeaveTangentAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, ref Vector3, ref Vector3, void> getLocationAndTangentAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, ref Vector3, void> getLocationAtDistanceAlongSpline;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, ref Vector3, void> getLocationAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, ref Vector3, void> getLocationAtTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, ref Vector3, void> getRightVectorAtDistanceAlongSpline;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, ref Vector3, void> getRightVectorAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, Bool, ref Vector3, void> getRightVectorAtTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, float> getRollAtDistanceAlongSpline;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, float> getRollAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, Bool, float> getRollAtTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, ref Quaternion, void> getRotationAtDistanceAlongSpline;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, ref Quaternion, void> getRotationAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, Bool, ref Quaternion, void> getRotationAtTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, ref Vector3, void> getScaleAtDistanceAlongSpline;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, ref Vector3, void> getScaleAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, Bool, ref Vector3, void> getScaleAtTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getSplineLength;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, Bool, Bool, ref Transform, void> getTransformAtTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, ref Vector3, void> getUpVectorAtDistanceAlongSpline;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplineCoordinateSpace, ref Vector3, void> getUpVectorAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, SplineCoordinateSpace, Bool, ref Vector3, void> getUpVectorAtTime;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setDuration;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, SplinePointType, Bool, void> setSplinePointType;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, Bool, void> setClosedLoop;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, SplineCoordinateSpace, void> setDefaultUpVector;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, in Vector3, SplineCoordinateSpace, Bool, void> setLocationAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, in Vector3, SplineCoordinateSpace, Bool, void> setTangentAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, in Vector3, in Vector3, SplineCoordinateSpace, Bool, void> setTangentsAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, in Vector3, SplineCoordinateSpace, Bool, void> setUpVectorAtSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, SplineCoordinateSpace, Bool, void> addSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, int, SplineCoordinateSpace, Bool, void> addSplinePointAtIndex;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> clearSplinePoints;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, SplineCoordinateSpace, ref Vector3, void> findDirectionClosestToWorldLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, SplineCoordinateSpace, ref Vector3, void> findLocationClosestToWorldLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, SplineCoordinateSpace, ref Vector3, void> findUpVectorClosestToWorldLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, SplineCoordinateSpace, ref Vector3, void> findRightVectorClosestToWorldLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, SplineCoordinateSpace, float> findRollClosestToWorldLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, ref Vector3, void> findScaleClosestToWorldLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, SplineCoordinateSpace, ref Vector3, void> findTangentClosestToWorldLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, in Vector3, SplineCoordinateSpace, Bool, ref Transform, void> findTransformClosestToWorldLocation;
		internal static delegate* unmanaged[Cdecl]<IntPtr, int, Bool, void> removeSplinePoint;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> updateSpline;
	}

	unsafe partial class RadialForceComponent {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getIgnoreOwningActor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getImpulseVelocityChange;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> getLinearFalloff;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getForceStrength;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getImpulseStrength;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float> getRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setIgnoreOwningActor;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setImpulseVelocityChange;
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool, void> setLinearFalloff;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setForceStrength;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setImpulseStrength;
		internal static delegate* unmanaged[Cdecl]<IntPtr, float, void> setRadius;
		internal static delegate* unmanaged[Cdecl]<IntPtr, CollisionChannel, void> addCollisionChannelToAffect;
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> fireImpulse;
	}

	unsafe partial class MaterialInterface {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isTwoSided;
	}

	unsafe partial class Material {
		internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> isDefaultMaterial;
	}

	unsafe partial class MaterialInstance {
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Bool> isChildOf;
		internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> getParent;
	}

	unsafe partial class MaterialInstanceDynamic {
		internal static delegate* unmanaged[Cdecl]<IntPtr, void> clearParameterValues;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], IntPtr, void> setTextureParameterValue;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], in LinearColor, void> setVectorParameterValue;
		internal static delegate* unmanaged[Cdecl]<IntPtr, byte[], float, void> setScalarParameterValue;
	}
}

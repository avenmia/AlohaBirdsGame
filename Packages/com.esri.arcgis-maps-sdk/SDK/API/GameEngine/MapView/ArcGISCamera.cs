// COPYRIGHT 1995-2025 ESRI
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States and applicable international
// laws, treaties, and conventions.
//
// For additional information, contact:
// Attn: Contracts and Legal Department
// Environmental Systems Research Institute, Inc.
// 380 New York Street
// Redlands, California 92373
// USA
//
// email: legal@esri.com
using System.Runtime.InteropServices;
using System;

namespace Esri.GameEngine.MapView
{
    /// <summary>
    /// A camera represents an observer's location and their perspective of an <see cref="GameEngine.Map.ArcGISMap">ArcGISMap</see>. It is used as a data loading point.
    /// </summary>
    /// <remarks>
    /// A <see cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</see> object can be thought of as a camera that you look through to see a viewable area of a scene. What
    /// you see depends on how you orientate the camera and how far it is above the <see cref="">Surface</see>. A camera has four main
    /// configurable properties:
    /// 
    /// * Location - The 3D point in space where the camera is located.
    /// * Heading - The angle around the z-axis the camera is rotated. The angle is clockwise from north in the East,
    ///   North, Up (ENU) ground reference frame. The value is between 0 to 360. 0 is looking North and 90 is looking
    ///   East.
    /// * Pitch - The angle around the y-axis the camera is rotated. The value is between 0 to 180. 0 is looking
    ///   straight down, 180 is looking straight up.
    /// * Roll - The angle around the x-axis the camera is rotated. The value is between 0 to 360. 0 is horizontal, 180
    ///   is upside down.
    /// 
    /// You can construct a <see cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</see> based on these values or you can obtain it from the scene view's current
    /// viewpoint using <see cref="">SceneView.getCurrentViewpointCamera()</see>. The <see cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</see> is immutable which means that
    /// once it is created it you cannot modify it.
    /// 
    /// You can define the user's camera interactions using the scene view's camera controller. The default camera
    /// controller (<see cref="">GlobeCameraController</see>) allows users to freely move and focus the camera anywhere in the scene.
    /// Other camera controllers provide specialized behavior, such as:
    /// 
    /// * The <see cref="">OrbitGeoElementCameraController</see> locks the camera to maintain focus on a (possibly moving) <see cref="">GeoElement</see>. 
    /// * The <see cref="">OrbitLocationCameraController</see> locks the camera to orbit and to maintain focus on a fixed location.
    /// </remarks>
    /// <since>1.0.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISCamera
    {
        #region Constructors
        /// <summary>
        /// Creates a camera with the specified location, heading, pitch, and roll.
        /// </summary>
        /// <param name="locationPoint">A point geometry containing the location and altitude at which to place the camera. If the altitude is below the <see cref="">Scene.baseSurface</see> and the <see cref="">Surface.navigationConstraint</see> is set to  <see cref="">NavigationConstraint.stayAbove</see>, the camera will be located at the <see cref="">Surface</see>. Note: that the default is <see cref="">NavigationConstraint.stayAbove</see>. If the point has a spatial reference, the point projects to WGS84. Otherwise, a point spatial reference of WGS84 is assumed.</param>
        /// <param name="heading">The angle around the z-axis the camera is rotated. The angle is clockwise from north in the East, North, Up (ENU) ground reference frame. The value is between 0 to 360. 0 is looking North and 90 is looking East. Values are wrapped around so that they fall within 0 to 360.</param>
        /// <param name="pitch">The angle around the y-axis the camera is rotated in the East, North, Up (ENU) ground reference frame. The value is between 0 to 180. 0 is looking straight down and 180 is looking straight up. A negative value defaults to 0 and a value greater than 180 defaults to 180. If the behavior of a negative pitch is required, then the corresponding transformation with positive pitch can be set instead. For example, if heading:0 pitch:-20 roll:0 is required then heading:180 pitch:20 roll:180 can be used instead.</param>
        /// <param name="roll">The angle around the x-axis the camera is rotated in the East, North, Up (ENU) ground reference frame. The value is between 0 to 360. 0 is horizontal, 180 is upside down. Values are wrapped so that they fall within 0 to 360.</param>
        /// <since>1.0.0</since>
        public ArcGISCamera(GameEngine.Geometry.ArcGISPoint locationPoint, double heading, double pitch, double roll)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localLocationPoint = locationPoint.Handle;
            
            Handle = PInvoke.RT_Camera_createWithLocationHeadingPitchRoll(localLocationPoint, heading, pitch, roll, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        #endregion // Constructors
        
        #region Properties
        /// <summary>
        /// The heading of the camera.
        /// </summary>
        /// <remarks>
        /// The angle around the z-axis the camera is rotated. The angle is clockwise from north in the East,
        /// North, Up (ENU) ground reference frame. The value is between 0 to 360. 0 is looking North and 90 is
        /// looking East.
        /// </remarks>
        /// <seealso cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</seealso>
        /// <since>1.0.0</since>
        public double Heading
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_Camera_getHeading(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// The point geometry containing the location and altitude of the camera.
        /// </summary>
        /// <seealso cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</seealso>
        /// <since>1.0.0</since>
        public GameEngine.Geometry.ArcGISPoint Location
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_Camera_getLocation(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                GameEngine.Geometry.ArcGISPoint localLocalResult = null;
                
                if (localResult != IntPtr.Zero)
                {
                    localLocalResult = new GameEngine.Geometry.ArcGISPoint(localResult);
                }
                
                return localLocalResult;
            }
        }
        
        /// <summary>
        /// The pitch of the camera.
        /// </summary>
        /// <remarks>
        /// The angle around the y-axis the camera is rotated in the East, North, Up (ENU) ground reference frame.
        /// The value is between 0 to 180. 0 is looking straight down and 180 is looking straight up.
        /// </remarks>
        /// <seealso cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</seealso>
        /// <since>1.0.0</since>
        public double Pitch
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_Camera_getPitch(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// The roll of the camera.
        /// </summary>
        /// <remarks>
        /// The angle around the x-axis the camera is rotated in the East, North, Up (ENU) ground reference frame.
        /// The value is between 0 to 360. 0 is horizontal, 180 is upside down.
        /// </remarks>
        /// <seealso cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</seealso>
        /// <since>1.0.0</since>
        public double Roll
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_Camera_getRoll(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Creates a copy of the camera with the change in altitude applied.
        /// </summary>
        /// <param name="deltaAltitude">The altitude delta to apply to the output camera.</param>
        /// <returns>
        /// A copy of the camera with an elevation delta adjusted by the parameter delta_altitude.
        /// </returns>
        /// <seealso cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</seealso>
        /// <since>1.0.0</since>
        public ArcGISCamera Elevate(double deltaAltitude)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_Camera_elevate(Handle, deltaAltitude, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            ArcGISCamera localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                localLocalResult = new ArcGISCamera(localResult);
            }
            
            return localLocalResult;
        }
        
        /// <summary>
        /// Tests if this object is equal to a second <see cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</see> object.
        /// </summary>
        /// <param name="otherCamera">The other camera object.</param>
        /// <returns>
        /// True if the comparison succeeds and the objects are equal, false otherwise.
        /// </returns>
        /// <seealso cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</seealso>
        /// <since>1.0.0</since>
        public bool Equals(ArcGISCamera otherCamera)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localOtherCamera = otherCamera.Handle;
            
            var localResult = PInvoke.RT_Camera_equals(Handle, localOtherCamera, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Creates a copy of the camera with a new location.
        /// </summary>
        /// <param name="location">The location to move the output camera to.</param>
        /// <returns>
        /// A copy of the camera with the location changed.
        /// </returns>
        /// <seealso cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</seealso>
        /// <since>1.0.0</since>
        public ArcGISCamera MoveTo(GameEngine.Geometry.ArcGISPoint location)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localLocation = location.Handle;
            
            var localResult = PInvoke.RT_Camera_moveTo(Handle, localLocation, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            ArcGISCamera localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                localLocalResult = new ArcGISCamera(localResult);
            }
            
            return localLocalResult;
        }
        
        /// <summary>
        /// Creates a copy of the camera with the specified heading, pitch and roll values.
        /// </summary>
        /// <param name="heading">The angle around the z-axis the new camera is rotated. The angle is clockwise from north in the East, North, Up (ENU) ground reference frame. The value is between 0 to 360. 0 is looking North and 90 is looking East. Values are wrapped around so that they fall within 0 to 360.</param>
        /// <param name="pitch">The angle around the y-axis the camera is rotated in the East, North, Up (ENU) ground reference frame. The value is between 0 to 180. 0 is looking straight down and 180 is looking straight up. A negative value defaults to 0 and a value greater than 180 defaults to 180. If the behavior of a negative pitch is required, then the corresponding transformation with positive pitch can be set instead. For example, if heading:0 pitch:-20 roll:0 is required then heading:180 pitch:20 roll:180 can be used instead.</param>
        /// <param name="roll">The angle around the x-axis the new camera is rotated in the East, North, Up (ENU) ground reference frame. The value is between 0 to 360. 0 is horizontal, 180 is upside down. Values are wrapped so that they fall within 0 to 360.</param>
        /// <returns>
        /// A copy of the camera with the position moved
        /// </returns>
        /// <seealso cref="GameEngine.MapView.ArcGISCamera">ArcGISCamera</seealso>
        /// <since>1.0.0</since>
        public ArcGISCamera RotateTo(double heading, double pitch, double roll)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_Camera_rotateTo(Handle, heading, pitch, roll, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            ArcGISCamera localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                localLocalResult = new ArcGISCamera(localResult);
            }
            
            return localLocalResult;
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISCamera(IntPtr handle) => Handle = handle;
        
        ~ArcGISCamera()
        {
            if (Handle != IntPtr.Zero)
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                PInvoke.RT_Camera_destroy(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
            }
        }
        
        internal IntPtr Handle { get; set; }
        
        public static implicit operator bool(ArcGISCamera other)
        {
            return other != null && other.Handle != IntPtr.Zero;
        }
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_Camera_createWithLocationHeadingPitchRoll(IntPtr locationPoint, double heading, double pitch, double roll, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern double RT_Camera_getHeading(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_Camera_getLocation(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern double RT_Camera_getPitch(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern double RT_Camera_getRoll(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_Camera_elevate(IntPtr handle, double deltaAltitude, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_Camera_equals(IntPtr handle, IntPtr otherCamera, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_Camera_moveTo(IntPtr handle, IntPtr location, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_Camera_rotateTo(IntPtr handle, double heading, double pitch, double roll, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_Camera_destroy(IntPtr handle, IntPtr errorHandle);
        #endregion // P-Invoke Declarations
    }
}
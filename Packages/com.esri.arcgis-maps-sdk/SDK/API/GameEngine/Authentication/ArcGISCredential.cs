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

namespace Esri.GameEngine.Authentication
{
    /// <summary>
    /// A base class for types of ArcGIS credentials used to access secured resources.
    /// </summary>
    /// <remarks>
    /// This is a base class for ArcGIS credentials requiring OAuth or ArcGIS Token authentication,
    /// such as <see cref="GameEngine.Authentication.ArcGISOAuthUserCredential">ArcGISOAuthUserCredential</see>, <see cref="">OAuthApplicationCredential</see>, <see cref="">PregeneratedTokenCredential</see>
    /// and <see cref="">TokenCredential</see>.
    /// </remarks>
    /// <since>1.1.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISCredential
    {
        #region Properties
        /// <summary>
        /// The type of the credential.
        /// </summary>
        /// <seealso cref="GameEngine.Authentication.ArcGISCredentialType">ArcGISCredentialType</seealso>
        internal ArcGISCredentialType ObjectType
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_ArcGISCredential_getObjectType(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// The referer used to generate a token.
        /// </summary>
        /// <remarks>
        /// * <see cref="">TokenCredential</see> - The referer is `app://arcgis-maps/<application identifier>`.
        /// * <see cref="">PregeneratedTokenCredential</see> - The referer passed to the constructor should match the referer
        /// used to generate the token, or empty string if none was used.
        /// * <see cref="GameEngine.Authentication.ArcGISOAuthUserCredential">ArcGISOAuthUserCredential</see> and <see cref="">OAuthApplicationCredential</see> - The referer is an empty string.
        /// </remarks>
        /// <since>1.1.0</since>
        public string Referer
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_ArcGISCredential_getReferer(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        
        /// <summary>
        /// The URL by which the root of a server is accessed.
        /// </summary>
        /// <remarks>
        /// This is the URL against which rest endpoints are resolved. For example,
        /// "https://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0"
        /// would have a server context of "https://sampleserver3.arcgisonline.com/ArcGIS", on which we could add
        /// "/rest/info" or "/rest" to fetch the server information.
        /// </remarks>
        /// <since>1.1.0</since>
        public string ServerContext
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_ArcGISCredential_getServerContext(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        
        /// <summary>
        /// The user associated with the credential.
        /// </summary>
        /// <since>1.1.0</since>
        public string Username
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_ArcGISCredential_getUsername(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Tests if the two <see cref="GameEngine.Authentication.ArcGISCredential">ArcGISCredential</see> objects are equal.
        /// </summary>
        /// <param name="other">The credential to compare to.</param>
        /// <returns>
        /// True if the comparison succeeds and the objects are equal, false otherwise.
        /// </returns>
        /// <since>1.1.0</since>
        public bool Equals(ArcGISCredential other)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localOther = other.Handle;
            
            var localResult = PInvoke.RT_ArcGISCredential_equals(Handle, localOther, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Gets the <see cref="GameEngine.Authentication.ArcGISTokenInfo">ArcGISTokenInfo</see> generated by this credential.
        /// </summary>
        /// <returns>
        /// A <see cref="Unity.ArcGISFuture<T>">ArcGISFuture<T></see> that returns an <see cref="GameEngine.Authentication.ArcGISTokenInfo">ArcGISTokenInfo</see> from this credential.
        /// </returns>
        internal Unity.ArcGISFuture<ArcGISTokenInfo> GetArcGISTokenInfoAsync()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_ArcGISCredential_getArcGISTokenInfoAsync(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            Unity.ArcGISFuture<ArcGISTokenInfo> localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                localLocalResult = new Unity.ArcGISFuture<ArcGISTokenInfo>(localResult);
            }
            
            return localLocalResult;
        }
        
        /// <summary>
        /// Indicates whether or not a token in the authorization header is supported.
        /// </summary>
        /// <returns>
        /// The return value is based on the version number of the server or portal. Sending
        /// tokens in headers requires either a full version of 10.5.1 or greater, or a
        /// current version of 5.1 or greater.
        /// </returns>
        internal bool IsTokenInHeaderSupported()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_ArcGISCredential_isTokenInHeaderSupported(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Indicates whether or not the credential is valid.
        /// </summary>
        /// <returns>
        /// True if the credential has a non-expired pregenerated token, or is capable of generating new tokens.
        /// </returns>
        internal bool IsValid()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_ArcGISCredential_isValid(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISCredential(IntPtr handle) => Handle = handle;
        
        ~ArcGISCredential()
        {
            if (Handle != IntPtr.Zero)
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                PInvoke.RT_ArcGISCredential_destroy(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
            }
        }
        
        internal IntPtr Handle { get; set; }
        
        public static implicit operator bool(ArcGISCredential other)
        {
            return other != null && other.Handle != IntPtr.Zero;
        }
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern ArcGISCredentialType RT_ArcGISCredential_getObjectType(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_ArcGISCredential_getReferer(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_ArcGISCredential_getServerContext(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_ArcGISCredential_getUsername(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_ArcGISCredential_equals(IntPtr handle, IntPtr other, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_ArcGISCredential_getArcGISTokenInfoAsync(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_ArcGISCredential_isTokenInHeaderSupported(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_ArcGISCredential_isValid(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_ArcGISCredential_destroy(IntPtr handle, IntPtr errorHandle);
        #endregion // P-Invoke Declarations
    }
}
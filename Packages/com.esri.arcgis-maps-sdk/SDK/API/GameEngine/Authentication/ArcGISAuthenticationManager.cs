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
using Esri.ArcGISMapsSDK.Authentication;
using Esri.ArcGISMapsSDK.Utils;
using Esri.GameEngine.Map;
using Esri.Unity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Esri.GameEngine.Authentication
{
	/// <summary>
	/// Manages requests for ArcGIS secure resources made by an application.
	/// </summary>
	/// <remarks>
	/// The authentication manager allows you to manage user credentials for accessing secured ArcGIS Online and ArcGIS
	/// Enterprise resources. You can obtain the <see cref="GameEngine.Authentication.ArcGISAuthenticationManager">ArcGISAuthenticationManager</see> from the <see cref="GameEngine.ArcGISRuntimeEnvironment.AuthenticationManager">ArcGISRuntimeEnvironment.AuthenticationManager</see>
	/// static property. The authentication manager provides a central place for you to configure authentication challenge
	/// handlers and credential stores:
	///
	/// * ArcGIS and network challenge handlers allow you to respond to authentication challenges. For example, you
	///   can write code in an authentication challenge handler to prompt the user for credential information, create a
	///   credential, and use it to continue with the challenge.
	/// * Credential stores are available for storing ArcGIS and network credentials that are automatically
	///   checked when your application attempts to connect to secured resources. These stores can also be persisted so
	///   that the user does not have to sign in again when the application is re-launched.
	///
	/// If your application attempts to access a secure resource, and there is no matching credential in the credential
	/// store, the app raises an authentication challenge. If the ArcGIS secured resource requires OAuth or ArcGIS Token
	/// authentication, an ArcGIS authentication challenge is raised. If the ArcGIS secured resource requires network
	/// credentials, such as Integrated Windows Authentication (IWA) or Public Key Infrastructure (PKI), network
	/// authentication challenge is raised.
	///
	/// To allow your app users to respond to the authentication challenge:
	///
	/// * Create an authentication challenge handler and pass it to the <see cref="GameEngine.Authentication.ArcGISAuthenticationManager">ArcGISAuthenticationManager</see>.
	/// * Customize the challenge handler to prompt the user for a username and password and use those values to create
	///   the appropriate type of <see cref="GameEngine.Authentication.ArcGISCredential">ArcGISCredential</see>.
	/// * Confirm that the credential permits access to the secure resource.
	///
	/// A successful credential, created while handling an authentication challenge, is automatically placed in the
	/// credential store of the <see cref="GameEngine.Authentication.ArcGISAuthenticationManager">ArcGISAuthenticationManager</see>. This store exists for the lifetime of the application and its
	/// credentials can be used when subsequent requests are made.
	///
	/// If you want to avoid prompting users for credentials between application sessions, you can persist the
	/// credential store. Ensure that the credentials are secured using an appropriate mechanism for your platform so
	/// they are not available to other apps or processes.
	///
	/// During user sign-out you should clear all credentials from the credential store to prevent users accessing
	/// resources for which they do not have permission.
	///
	/// See the ArcGIS for Developers website for an overview of <see cref="Security and authentication">https://developers.arcgis.com/documentation/security-and-authentication/</see>.
	/// </remarks>
	/// <since>1.1.0</since>
	[StructLayout(LayoutKind.Sequential)]
	public partial class ArcGISAuthenticationManager
	{
		#region Constructors
		/// <summary>
		/// Creates an AuthenticationManager.
		/// </summary>
		internal ArcGISAuthenticationManager()
		{
			var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

			Handle = PInvoke.RT_AuthenticationManager_create(errorHandler);

			Unity.ArcGISErrorManager.CheckError(errorHandler);

			Initialize();
		}
		#endregion Constructors

		#region Properties
		/// <summary>
		/// The handler is called to handle <see cref="GameEngine.Authentication.ArcGISAuthenticationChallenge">ArcGISAuthenticationChallenge</see> when requests are made for ArcGIS secured
		/// resources that require an OAuth or ArcGIS token authentication.
		/// </summary>
		/// <remarks>
		/// The credential provided while handling an authentication challenge is placed in the ArcGIS credential store of the
		/// <see cref="GameEngine.ArcGISRuntimeEnvironment.AuthenticationManager">ArcGISRuntimeEnvironment.AuthenticationManager</see> and used by all subsequent requests that have a matching server context.
		/// </remarks>
		/// <since>1.1.0</since>
		public ArcGISAuthenticationChallengeHandler AuthenticationChallengeHandler
		{
			get
			{
				return _authenticationChallengeHandler;
			}
			set
			{
				if (_authenticationChallengeHandler != value)
				{
					_authenticationChallengeHandler = value;
				}
			}
		}

		/// <summary>
		/// Storage for <see cref="GameEngine.Authentication.ArcGISCredential">ArcGISCredential</see> objects. The credential store is checked for a matching credential before
		/// sending requests or issuing authentication challenges.
		/// </summary>
		/// <since>1.1.0</since>
		public ArcGISCredentialStore CredentialStore
		{
			get
			{
				var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				var localResult = PInvoke.RT_AuthenticationManager_getArcGISCredentialStore(Handle, errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);

				ArcGISCredentialStore localLocalResult = null;

				if (localResult != IntPtr.Zero)
				{
					localLocalResult = new ArcGISCredentialStore(localResult);
				}

				return localLocalResult;
			}
			set
			{
				var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				var localValue = value.Handle;

				PInvoke.RT_AuthenticationManager_setArcGISCredentialStore(Handle, localValue, errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}
		}

		/// <summary>
		/// Collection of active <see cref="">UserConfiguration</see>`s.
		/// </summary>
		public static List<ArcGISOAuthUserConfiguration> OAuthUserConfigurations = new List<ArcGISOAuthUserConfiguration>();

		/// <summary>
		/// Used for handling <see cref="">ArcGISOAuthUserLoginPrompt</see>.
		/// </summary>
		/// <remarks>
		/// The instance should implement <see cref="">ArcGISOAuthUserLoginPromptHandler</see>.
		/// </remarks>
		private static ArcGISOAuthUserLoginPromptHandler oauthUserLoginPromptHandler = null;
		public ArcGISOAuthUserLoginPromptHandler OAuthUserLoginPromptHandler
		{
			get => oauthUserLoginPromptHandler;
			set
			{
				oauthUserLoginPromptHandler = value;
			}
		}

		/// <summary>
		/// The unique app or bundle ID that should be appended to the default referer used by <see cref="">TokenCredential</see> instances.
		/// </summary>
		/// <remarks>
		/// This should be set before any <see cref="">TokenCredential</see> instances are created.
		/// This only applies to <see cref="">TokenCredential.referer</see>, which is app://arcgis-maps/
		/// </remarks>
		internal string RefererAppId
		{
			get
			{
				var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				var localResult = PInvoke.RT_AuthenticationManager_getRefererAppId(Handle, errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);

				return Unity.Convert.FromArcGISString(localResult);
			}
			set
			{
				var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				PInvoke.RT_AuthenticationManager_setRefererAppId(Handle, value, errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}
		}

		/// <summary>
		/// The maximum number of times a request can be retried.
		/// </summary>
		internal static byte RequestRetryCount
		{
			get
			{
				var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				var localResult = PInvoke.RT_AuthenticationManager_getRequestRetryCount(errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);

				return localResult;
			}
		}
		#endregion Properties

		#region Events
		/// <summary>
		/// Sets the global callback invoked when an authentication challenge is issued.
		/// </summary>
		internal ArcGISAuthenticationManagerIssuedChallengeEvent ArcGISAuthenticationChallengeIssued
		{
			get
			{
				return _authenticationChallengeIssuedHandler.Delegate;
			}
			set
			{
				if (_authenticationChallengeIssuedHandler.Delegate == value)
				{
					return;
				}

				var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				if (value != null)
				{
					_authenticationChallengeIssuedHandler.Delegate = value;

					PInvoke.RT_AuthenticationManager_setArcGISAuthenticationChallengeIssuedCallback(Handle, ArcGISAuthenticationManagerIssuedChallengeEventHandler.HandlerFunction, _authenticationChallengeIssuedHandler.UserData, errorHandler);
				}
				else
				{
					PInvoke.RT_AuthenticationManager_setArcGISAuthenticationChallengeIssuedCallback(Handle, null, _authenticationChallengeIssuedHandler.UserData, errorHandler);

					_authenticationChallengeIssuedHandler.Dispose();
				}

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}
		}

		/// <summary>
		/// Sets the global callback invoked when an OAuth user login is issued.
		/// </summary>
		internal ArcGISAuthenticationManagerOAuthUserLoginEvent OAuthUserLogin
		{
			get
			{
				return _oauthUserLoginHandler.Delegate;
			}
			set
			{
				if (_oauthUserLoginHandler.Delegate == value)
				{
					return;
				}

				var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				if (value != null)
				{
					_oauthUserLoginHandler.Delegate = value;

					PInvoke.RT_AuthenticationManager_setOAuthUserLoginCallback(Handle, ArcGISAuthenticationManagerOAuthUserLoginEventHandler.HandlerFunction, _oauthUserLoginHandler.UserData, errorHandler);
				}
				else
				{
					PInvoke.RT_AuthenticationManager_setOAuthUserLoginCallback(Handle, null, _oauthUserLoginHandler.UserData, errorHandler);

					_oauthUserLoginHandler.Dispose();
				}

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}
		}

		/// <summary>
		/// Sets the global callback invoked when a portal network secured inquiry is issued.
		/// </summary>
		internal ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEvent PortalNetworkSecuredInquiry
		{
			get
			{
				return _portalNetworkSecuredInquiryHandler.Delegate;
			}
			set
			{
				if (_portalNetworkSecuredInquiryHandler.Delegate == value)
				{
					return;
				}

				var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				if (value != null)
				{
					_portalNetworkSecuredInquiryHandler.Delegate = value;

					PInvoke.RT_AuthenticationManager_setPortalNetworkSecuredInquiryCallback(Handle, ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEventHandler.HandlerFunction, _portalNetworkSecuredInquiryHandler.UserData, errorHandler);
				}
				else
				{
					PInvoke.RT_AuthenticationManager_setPortalNetworkSecuredInquiryCallback(Handle, null, _portalNetworkSecuredInquiryHandler.UserData, errorHandler);

					_portalNetworkSecuredInquiryHandler.Dispose();
				}

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}
		}
		#endregion Events

		#region Internal Members
		internal ArcGISAuthenticationManager(IntPtr handle) => Handle = handle;

		~ArcGISAuthenticationManager()
		{
			if (Handle != IntPtr.Zero)
			{
				if (_authenticationChallengeIssuedHandler.Delegate != null)
				{
					PInvoke.RT_AuthenticationManager_setArcGISAuthenticationChallengeIssuedCallback(Handle, null, _authenticationChallengeIssuedHandler.UserData, IntPtr.Zero);

					_authenticationChallengeIssuedHandler.Dispose();
				}

				if (_oauthUserLoginHandler.Delegate != null)
				{
					PInvoke.RT_AuthenticationManager_setOAuthUserLoginCallback(Handle, null, _oauthUserLoginHandler.UserData, IntPtr.Zero);

					_oauthUserLoginHandler.Dispose();
				}

				if (_portalNetworkSecuredInquiryHandler.Delegate != null)
				{
					PInvoke.RT_AuthenticationManager_setPortalNetworkSecuredInquiryCallback(Handle, null, _portalNetworkSecuredInquiryHandler.UserData, IntPtr.Zero);

					_portalNetworkSecuredInquiryHandler.Dispose();
				}

				var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				PInvoke.RT_AuthenticationManager_destroy(Handle, errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}
		}

		internal IntPtr Handle { get; set; }

		public static implicit operator bool(ArcGISAuthenticationManager other)
		{
			return other != null && other.Handle != IntPtr.Zero;
		}

		internal ArcGISAuthenticationChallengeHandler _authenticationChallengeHandler = null;

		internal ArcGISAuthenticationManagerIssuedChallengeEventHandler _authenticationChallengeIssuedHandler = new ArcGISAuthenticationManagerIssuedChallengeEventHandler();

		internal ArcGISAuthenticationManagerOAuthUserLoginEventHandler _oauthUserLoginHandler = new ArcGISAuthenticationManagerOAuthUserLoginEventHandler();

		internal ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEventHandler _portalNetworkSecuredInquiryHandler = new ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEventHandler();
		#endregion Internal Members

		#region Private Members
		private static Queue<ArcGISAuthenticationChallenge> authenticationChallenges = new Queue<ArcGISAuthenticationChallenge>();
		private static ArcGISFuture<ArcGISOAuthUserCredential> pendingCredentialTasks = null;
		private static bool challengeIsBeingHandled = false;

#if UNITY_EDITOR
		private void Deinitialize()
		{
			AssemblyReloadEvents.beforeAssemblyReload -= Deinitialize;

			OAuthUserLoginPromptHandler?.Dispose();

			pendingCredentialTasks = null;
			challengeIsBeingHandled = false;
			while (authenticationChallenges.Count > 0)
			{
				var challenge = authenticationChallenges.Dequeue();

				challenge.Cancel();
			}

			CredentialStore.RemoveAll();

			PortalNetworkSecuredInquiry = null;
			ArcGISAuthenticationChallengeIssued = null;
			OAuthUserLogin = null;
		}
#endif

		private void Initialize()
		{
#if UNITY_EDITOR
			AssemblyReloadEvents.beforeAssemblyReload += Deinitialize;
#endif
			PortalNetworkSecuredInquiry += OnPortalNetworkSecuredInquiryIssued;

			ArcGISAuthenticationChallengeIssued += OnAuthenticationChallengeIssued;

			OAuthUserLogin += OnOAuthUserLoginIssued;
		}

		private void OnPortalNetworkSecuredInquiryIssued(ArcGISPortalNetworkSecuredInquiry inquiry)
		{
			inquiry.Respond(false);
		}

		private void OnAuthenticationChallengeIssued(ArcGISAuthenticationChallenge challenge)
		{
			authenticationChallenges.Enqueue(challenge);

			if (challengeIsBeingHandled)
			{
				return;
			}

			HandleNextChallengeInQueue();
		}

		private void OnOAuthUserLoginIssued(ArcGISOAuthUserLoginPrompt prompt)
		{
			var response = prompt.RedirectURL + "?error=No handler provided for OAuthUserLogin.";

			if (OAuthUserLoginPromptHandler != null)
			{
				response = OAuthUserLoginPromptHandler.HandleOAuthUserLoginPrompt(prompt.AuthorizeURL, prompt.RedirectURL);
			}

			prompt.Respond(response);
		}

		private void HandleNextChallengeInQueue()
		{
			if (authenticationChallenges.Count == 0)
			{
				challengeIsBeingHandled = false;
				return;
			}

			challengeIsBeingHandled = true;

			ArcGISMainThreadScheduler.Instance().Schedule(() =>
			{
				HandleAuthenticationIssuedChallenge(authenticationChallenges.Peek());
			});
		}

		private void ApplyOAuthUserCredentialToQueue(ArcGISAuthenticationChallenge challenge, ArcGISFuture<ArcGISOAuthUserCredential> credentialTask)
		{
			var challenges = authenticationChallenges.ToArray();
			authenticationChallenges.Clear();

			var taskError = credentialTask.GetError();

			foreach (var nextChallenge in challenges)
			{
				if (challenge.IsMatch(nextChallenge))
				{
					if (taskError == null)
					{
						nextChallenge.ContinueWithCredential(credentialTask.Get());
					}
					else
					{
						UnityEngine.Debug.LogError("Authentication Error | " + taskError.Message);

						if (taskError.Message.Contains("canceled"))
						{
							nextChallenge.Cancel();
						}
						else
						{
							nextChallenge.ContinueAndFail();
						}
					}
				}
				else
				{
					authenticationChallenges.Enqueue(nextChallenge);
				}
			}

			pendingCredentialTasks = null;

			HandleNextChallengeInQueue();
		}

		private void HandleAuthenticationIssuedChallenge(ArcGISAuthenticationChallenge authenticationChallenge)
		{
			var configuration = GetOAuthAuthenticationConfigurationForUrl(authenticationChallenge.RequestURL);

			if (configuration == null)
			{
				if (_authenticationChallengeHandler != null)
				{
					_authenticationChallengeHandler.HandleArcGISAuthenticationChallenge(authenticationChallenge);
				}
				else
				{
					UnityEngine.Debug.LogWarning("A valid API Key or OAuth User Configuration is required for " + authenticationChallenge.RequestURL);
					authenticationChallenge.ContinueAndFail();
					authenticationChallenges.Dequeue();
				}

				HandleNextChallengeInQueue();
				return;
			}

			var oauthUserConfiguration = new ArcGISOAuthUserConfiguration(
				authenticationChallenge.RequestURL, configuration.ClientId, configuration.RedirectURL, "", 0, 0, 0, true,
				ArcGISUserInterfaceStyle.Unspecified, false);

			pendingCredentialTasks = ArcGISOAuthUserCredential.CreateAsync(oauthUserConfiguration);

			pendingCredentialTasks.TaskCompleted += () =>
			{
				ApplyOAuthUserCredentialToQueue(authenticationChallenge, pendingCredentialTasks);
			};
		}

		private ArcGISOAuthUserConfiguration GetOAuthAuthenticationConfigurationForUrl(string requestUrl)
		{
			if (OAuthUserConfigurations == null)
			{
				return null;
			}

			foreach (var configuration in OAuthUserConfigurations)
			{
				var portalContext = ArcGISURLUtils.GetServerContext(configuration.PortalURL);
				var requestContext = ArcGISURLUtils.GetServerContext(requestUrl);

				if (portalContext == requestContext && !string.IsNullOrEmpty(configuration.ClientId) && !string.IsNullOrEmpty(configuration.RedirectURL))
				{
					return configuration;
				}
			}

			return null;
		}
		#endregion Private Members
	}

	internal static partial class PInvoke
	{
		#region P-Invoke Declarations
		[DllImport(Unity.Interop.Dll)]
		internal static extern IntPtr RT_AuthenticationManager_create(IntPtr errorHandler);

		[DllImport(Unity.Interop.Dll)]
		internal static extern IntPtr RT_AuthenticationManager_getArcGISCredentialStore(IntPtr handle, IntPtr errorHandler);

		[DllImport(Unity.Interop.Dll)]
		internal static extern void RT_AuthenticationManager_setArcGISCredentialStore(IntPtr handle, IntPtr arcGISCredentialStore, IntPtr errorHandler);

		[DllImport(Unity.Interop.Dll)]
		internal static extern IntPtr RT_AuthenticationManager_getRefererAppId(IntPtr handle, IntPtr errorHandler);

		[DllImport(Unity.Interop.Dll)]
		internal static extern void RT_AuthenticationManager_setRefererAppId(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string refererAppId, IntPtr errorHandler);

		[DllImport(Unity.Interop.Dll)]
		internal static extern byte RT_AuthenticationManager_getRequestRetryCount(IntPtr errorHandler);

		[DllImport(Unity.Interop.Dll)]
		internal static extern void RT_AuthenticationManager_setArcGISAuthenticationChallengeIssuedCallback(IntPtr handle, ArcGISAuthenticationManagerIssuedChallengeEventInternal arcGISAuthenticationChallengeIssued, IntPtr userData, IntPtr errorHandler);

		[DllImport(Unity.Interop.Dll)]
		internal static extern void RT_AuthenticationManager_setOAuthUserLoginCallback(IntPtr handle, ArcGISAuthenticationManagerOAuthUserLoginEventInternal OAuthUserLogin, IntPtr userData, IntPtr errorHandler);

		[DllImport(Unity.Interop.Dll)]
		internal static extern void RT_AuthenticationManager_setPortalNetworkSecuredInquiryCallback(IntPtr handle, ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEventInternal portalNetworkSecuredInquiry, IntPtr userData, IntPtr errorHandler);

		[DllImport(Unity.Interop.Dll)]
		internal static extern void RT_AuthenticationManager_destroy(IntPtr handle, IntPtr errorHandle);
		#endregion P-Invoke Declarations
	}
}

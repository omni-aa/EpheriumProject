using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Mirror;

namespace uMMORPG
{

    public class NetworkAuthenticatorMMO : NetworkAuthenticator
    {
        [Header("Components")]
        public NetworkManagerMMO manager;

        // login info for the local player
        // we don't just name it 'account' to avoid collisions in handshake
        [Header("Login")]
        public string loginAccount = "";
        public string loginPassword = "";

#if _iMMOREGISTERMOBILE
    public string guestUserUniqID;
    public string googleSignInToken;

    public LoginType loginType; // 0 login/pass , 1 GuestUSer, 2 Google SignIn
#endif

        [Header("Security")]
        public string passwordSalt = "at_least_16_byte";
        public int accountMaxLength = 16;

        // client //////////////////////////////////////////////////////////////////
        public override void OnStartClient()
        {
            // register login success message, allowed before authenticated
            NetworkClient.RegisterHandler<LoginSuccessMsg>(OnClientLoginSuccess, false);
        }

        public override void OnClientAuthenticate()
        {
            // send login packet with hashed password, so that the original one
            // never leaves the player's computer.
            //
            // it's recommended to use a different salt for each hash. ideally we
            // would store each user's salt in the database. to not overcomplicate
            // things, we will use the account name as salt (at least 16 bytes)
            //
            // Application.version can be modified under:
            // Edit -> Project Settings -> Player -> Bundle Version
#if _iMMOREGISTERMOBILE
        if (loginType == LoginType.Guest)
        {
            // 1 ) if faut check si le guestUserUniqID à un compte associer
            //string account = Database.singleton.isRegistered_user(loginType, guestUserUniqID);
            Debug.Log("OnClientAuthenticate() Guest >>" + guestUserUniqID);
            loginPassword = guestUserUniqID;
            loginAccount = guestUserUniqID;

        }
        else if (loginType == LoginType.Goolge)
        {
            // Is a google login
            Debug.Log("OnClientAuthenticate() Google >>" + googleSignInToken);
            //string account = Database.singleton.isRegistered_user(loginType, googleSignInToken, manager.networkManagerMMORegisterMobile.WebClientID);
            loginPassword = manager.networkManagerMMORegisterMobile.WebClientID;
            loginAccount = googleSignInToken;
        }
#endif

            string hash = Utils.PBKDF2Hash(loginPassword, passwordSalt + loginAccount);
#if _iMMOREGISTERMOBILE
        LoginMsg message = new LoginMsg { account = loginAccount, password = hash, loginType = loginType, version = Application.version };
#else
            LoginMsg message = new LoginMsg { account = loginAccount, password = hash, version = Application.version };
#endif

            NetworkClient.connection.Send(message);
            Debug.Log("login message was sent");

            // set state
            manager.state = NetworkState.Handshake;
        }

        void OnClientLoginSuccess(LoginSuccessMsg msg)
        {
            // authenticated successfully. OnClientConnected will be called.
            OnClientAuthenticated.Invoke();
        }

        // server //////////////////////////////////////////////////////////////////
        public override void OnStartServer()
        {
            // register login message, allowed before authenticated
            NetworkServer.RegisterHandler<LoginMsg>(OnServerLogin, false);
        }

        public override void OnServerAuthenticate(NetworkConnectionToClient conn)
        {
            // wait for LoginMsg from client
        }

        // virtual in case someone wants to modify
        public virtual bool IsAllowedAccountName(string account)
        {
            // not too long?
            // only contains letters, number and underscore and not empty (+)?
            // (important for database safety etc.)
            return account.Length <= accountMaxLength &&
                   Regex.IsMatch(account, @"^[a-zA-Z0-9_]+$");
        }

        bool AccountLoggedIn(string account)
        {
            // in lobby or in world?
            return manager.lobby.ContainsValue(account) ||
                   Player.onlinePlayers.Values.Any(p => p.account == account)
#if _iMMOLOBBY && _MYSQL
               || (manager.networkManagerMMONetworkZones != null && Database.singleton.IsLogged(account))
#endif
                   ;
        }

        void OnServerLogin(NetworkConnectionToClient conn, LoginMsg message)
        {
#if _SERVER
            // correct version?
            if (message.version == Application.version)
            {
#if _iMMOREGISTERMOBILE && _SERVER
            // Classic login
            if (message.loginType == LoginType.Classique) { }
            // Guest user
            else if (message.loginType == LoginType.Guest)
            {
                string account = Database.singleton.isRegistered_user((int)message.loginType, message.account);
                message.account = account;

                Debug.Log("ok : " + account);
                string hash = Utils.PBKDF2Hash(account, passwordSalt + account);
                message.password = hash;
            }
            // Google user
            else if (message.loginType == LoginType.Goolge)
            {
                string account = Database.singleton.isRegistered_user((int)message.loginType, message.account, manager.networkManagerMMORegisterMobile.WebClientID);
                message.account = account;
                string hash = Utils.PBKDF2Hash(account, passwordSalt + manager.networkManagerMMORegisterMobile.WebClientID);
                message.password = hash;
            }
            Debug.Log(" Login type >> " + loginType);
#endif
                // allowed account name?
                if (IsAllowedAccountName(message.account))
                {


                    // validate account info
                    if (Database.singleton.TryLogin(message.account, message.password))
                    {
                        // not in lobby and not in world yet?
                        if (!AccountLoggedIn(message.account))
                        {
#if _iMMOLOBBY && _MYSQL
                        if (manager.networkManagerMMONetworkZones != null) // if networkmanagermmo have networkManagerMMONetworkZones referenced
                            Database.singleton.LogoutAccountUpdate(message.account, true);
#endif

                            // add to logged in accounts
                            manager.lobby[conn] = message.account;

                            // login successful
                            Debug.Log("login successful: " + message.account);

                            // notify client about successful login. otherwise it
                            // won't accept any further messages.
                            conn.Send(new LoginSuccessMsg());

                            // authenticate on server
                            OnServerAuthenticated.Invoke(conn);
                        }
                        else
                        {
                            //Debug.Log("account already logged in: " + message.account); <- don't show on live server
                            manager.ServerSendError(conn, "already logged in", true);

                            // note: we should disconnect the client here, but we can't as
                            // long as unity has no "SendAllAndThenDisconnect" function,
                            // because then the error message would never be sent.
                            //conn.Disconnect();
                        }
                    }
                    else
                    {
                        //Debug.Log("invalid account or password for: " + message.account); <- don't show on live server
                        manager.ServerSendError(conn, "invalid account", true);
                    }
                }
                else
                {
                    //Debug.Log("account name not allowed: " + message.account); <- don't show on live server
                    manager.ServerSendError(conn, "account name \"" + message.account + "\" not allowed", true);
                }
            }
            else
            {
                //Debug.Log("version mismatch: " + message.account + " expected:" + Application.version + " received: " + message.version); <- don't show on live server
                manager.ServerSendError(conn, "outdated version", true);
            }
#endif
        }
    }
}
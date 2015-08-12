using Microsoft.Framework.Configuration;
using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dentoncitycouncil {
    public static class Application {
        private static string _queueName, _queueKey, _helpelect_donate_to, _helpelect_support_to;
        private static string _stripeSecretKey, _stripePublicKey;
        private static string _path;
        public static void LoadFromEnvironment(IApplicationEnvironment env) {
            _path = env.ApplicationBasePath;
        }
        public static void LoadFromConfig(IConfiguration Configuration) {
            _queueName = Configuration.Get("queue:name");
            _queueKey = Configuration.Get("queue:key");
            _stripeSecretKey = Configuration.Get("stripe:secretkey");
            _stripePublicKey = Configuration.Get("stripe:publickey");
            _helpelect_donate_to = Configuration.Get("helpelect:donate:confirmationemail:to");
            _helpelect_support_to = Configuration.Get("helpelect:support:email:to");
        }

        public const string Title = "Denton for City Council";

        public static class Environment {
            public static string ApplicationBasePath { get { return _path; } }
        }

        public static class HelpElect {
            public static class Donate {
                public static string ConfirmationEmail { get { return _helpelect_donate_to; } }
            }
            public static class Support {
                public static string Email { get { return _helpelect_support_to; } }
            }
        }

        public static class Queue {
            public static string Name { get { return _queueName; } }
            public static string Key { get { return _queueKey; } }
        }
        public static class Stripe {
            public static string SecretKey { get { return _stripeSecretKey; } }
            public static string PublicKey { get { return _stripePublicKey; } }
        }
    }
}

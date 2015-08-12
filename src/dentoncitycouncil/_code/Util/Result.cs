using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Util {
    public class Result {
        protected bool _success;
        protected string _message;
        public Result(bool Success, string Message = null) {
            _success = Success;
            _message = Message;
        }
        public bool Success {
            get { return _success; }
        }
        public string Message {
            get { return _message; }
        }
    }
    public class Result<T> : Result {
        private T _o;
        public Result(bool Success) : base(Success) {
        }
        public Result(bool Success, string Message) : base(Success, Message) {
        }
        public Result(T o, bool Success) : base(Success) {
            _o = o;
        }
        public Result(T o, bool Success, string Message) : base(Success, Message) {
            _o = o;
        }
        public T Value {
            get { return _o; }
        }
    }
}

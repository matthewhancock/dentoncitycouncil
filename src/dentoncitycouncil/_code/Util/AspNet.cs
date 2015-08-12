using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Util.AspNet {
    public static class Middleware {
        public static IApplicationBuilder ForceHttps(this IApplicationBuilder App) {
            return App.Use(async (Context, Next) => {
                if(Context.Request.IsHttps) {
                    await Next();
                } else {
                    if (Context.Request.Method == Util.Http.Method.Get) {
                        Context.Response.Redirect("https://" + Context.Request.Host + Context.Request.Path, true);
                    } else {
                        Context.Response.StatusCode = Util.Http.StatusCode.BadRequest;
                        Context.Response.Body.Close();
                    }
                }
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lotereya.Controllers
{
    public class CustomController : Controller
    {
        public Models.ErrorCatcher _ErrorChatcher = null;
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            _ErrorChatcher = new Models.ErrorCatcher(true);
            return base.BeginExecute(requestContext, callback, state);
        }
	}
}
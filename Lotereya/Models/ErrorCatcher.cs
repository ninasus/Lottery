using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotereya.Models
{
    public class ErrorCatcher
    {
        private bool isShowingRealError { get; set; }
        public ErrorCatcher(bool _isShowingRealError)
        {
            isShowingRealError = _isShowingRealError;
        }

        public string ShowError(string realError, string stub)
        {
            return (isShowingRealError ? realError : stub);
        }
    }
}
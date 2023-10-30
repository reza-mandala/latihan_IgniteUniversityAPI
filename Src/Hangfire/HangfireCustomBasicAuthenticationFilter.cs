using Hangfire.Dashboard;
using System.Text;

namespace MyIgniteApi.Hangfire
{
    public class HangfireCustomBasicAuthenticationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _username;
        private readonly string _password;

        public HangfireCustomBasicAuthenticationFilter(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var header = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(header))
            {
                return false;
            }

            var authValues = Encoding.ASCII.GetString(Convert.FromBase64String(header.ToString().Split(' ')[1])).Split(':');
            var username = authValues[0];
            var password = authValues[1];

            return username.Equals(_username) && password.Equals(_password);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace example
{
    public partial class Default : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            LineProvider line = new LineProvider(
            ChannelID: "Your Channel ID"
            , Secret: "Your Channel Secret");

            string code = Request.QueryString["code"];

            if (!string.IsNullOrEmpty(code))
            {
                // All the Item1 are means http status code 200
                // Item2 content the information
                var RetrieveAccessToken = await line.RetrieveAccessToken(code, "Authentication domain of your channel");
                if (RetrieveAccessToken.Item1)
                {
                    string accessToken = RetrieveAccessToken.Item2.access_token;
                    string refreshToken = RetrieveAccessToken.Item2.refresh_token;
                    var RetrieveProfile = await line.RetrieveProfile(accessToken);
                    if (RetrieveAccessToken.Item1)
                    {
                        Literal1.Text = $@"<img src=""{RetrieveProfile.Item2.pictureUrl}/large"" alt=""*"" />";
                        Label1.Text = $"Hello! {RetrieveProfile.Item2.displayName}<br />{RetrieveProfile.Item2.statusMessage}<br />";

                        var ValidityAccessToken = await line.ValidityAccessToken(accessToken);
                        if (ValidityAccessToken.Item1)
                        {
                            Label1.Text += $"AccessToken not expire<br />";
                        }
                        else
                        {
                            var RetrievedReissuingAccessToken = await line.RetrievedReissuingAccessToken(accessToken, refreshToken);
                            if (RetrievedReissuingAccessToken.Item1)
                            {
                                accessToken = RetrievedReissuingAccessToken.Item2.accessToken;
                                refreshToken = RetrievedReissuingAccessToken.Item2.refreshToken;
                                Label1.Text += $"AccessToken Reissuing!<br />";
                            }
                        }

                        var Logout = await line.Logout(accessToken);
                        if (Logout)
                            Label1.Text += "Bye Bye";
                    }
                }
            }
        }
    }
}
using Authing.ApiClient;
using Authing.ApiClient.Types;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace AuthingApiClientTest
{
    public class TestAuthenticationClient
    {
        private AuthenticationClient client;

        private string RandomString()
        {
            return new Random().Next().ToString();
        }

        [SetUp]
        public void Setup()
        {
            client = new AuthenticationClient("59f86b4832eb28071bdd9214")
            {
                Host = "http://192.168.50.64:3000",
                PublicKey = @"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC4xKeUgQ+Aoz7TLfAfs9+paePb
5KIofVthEopwrXFkp8OCeocaTHt9ICjTT2QeJh6cZaDaArfZ873GPUn00eOIZ7Ae
+TiA2BKHbCvloW3w5Lnqm70iSsUi5Fmu9/2+68GZRH9L7Mlh8cFksCicW2Y2W2uM
GKl64GDcIq3au+aqJQIDAQAB
-----END PUBLIC KEY-----",
            };
        }

        [Test]
        public async Task GetCurrentUser()
        {
            var phone = "17611161550";
            var password = "123456";
            await client.LoginByPhonePassword(phone, password);
            var user = await client.GetCurrentUser();
            Assert.AreEqual(phone, user.Phone);
        }

        [Test]
        public async Task RegisterByEmail()
        {
            var email = RandomString() + "@gmail.com";
            var password = "123456";
            var user = await client.RegisterByEmail(email, password);
            Assert.AreEqual(email, user.Email);
        }

        [Test]
        public async Task RegisterByUsername()
        {
            var username = RandomString();
            var password = "123456";
            var user = await client.RegisterByUsername(username, password);
            Assert.AreEqual(username, user.Username);
        }

        [Test]
        public async Task RegisterByPhoneCode()
        {
            var phone = "17611161550";
            var code = "8942";
            var password = "123456";
            var user = await client.RegisterByPhoneCode(phone, code, password);
            Assert.AreEqual(phone, user.Phone);
        }

        [Test]
        public async Task SendSmsCode()
        {
            var phone = "17611161550";
            await client.SendSmsCode(phone);
        }

        [Test]
        public async Task LoginByEmail()
        {
            var email = RandomString() + "@gmail.com";
            var password = "123456";
            await client.RegisterByEmail(email, password);
            var user = await client.LoginByEmail(email, password);
            Assert.AreEqual(email, user.Email);
        }

        [Test]
        public async Task LoginByUsername()
        {
            var username = RandomString();
            var password = "123456";
            await client.RegisterByUsername(username, password);
            var user = await client.LoginByUsername(username, password);
            Assert.AreEqual(username, user.Username);
        }

        [Test]
        public async Task LoginByPhoneCode()
        {
            var phone = "17611161550";
            var code = "6579";
            var user = await client.LoginByPhoneCode(phone, code);
            Assert.AreEqual(phone, user.Phone);
        }

        [Test]
        public async Task LoginByPhonePassword()
        {
            var phone = "17611161550";
            var password = "123456";
            var user = await client.LoginByPhonePassword(phone, password);
            Assert.AreEqual(phone, user.Phone);
        }

        [Test]
        public async Task CheckLoginStatus()
        {
            var phone = "17611161550";
            var password = "123456";
            await client.LoginByPhonePassword(phone, password);
            var status = await client.CheckLoginStatus();
            Assert.AreEqual(status.Code, 200);
        }

        [Test]
        public async Task SendEmail()
        {
            var message = await client.SendEmail("1498881550@qq.com", EmailScene.RESET_PASSWORD);
            Assert.AreEqual(message.Code, 200);
        }

        [Test]
        public async Task ResetPasswordByPhoneCode()
        {
            var phone = "17611161550";
            var code = "1234";
            var password = "123456";
            var message = await client.ResetPasswordByPhoneCode(phone, code, password);
            Assert.AreEqual(message.Code, 200);
        }

        [Test]
        public async Task ResetPasswordByEmailCode()
        {
            var email = "1498881550@qq.com";
            var code = "1234";
            var password = "123456";
            var message = await client.ResetPasswordByEmailCode(email, code, password);
            Assert.AreEqual(message.Code, 200);
        }

        [Test]
        public async Task UpdateProfile()
        {
            var phone = "17611161550";
            var password = "123456";
            var nickname = RandomString();
            await client.LoginByPhonePassword(phone, password);
            var user = await client.UpdateProfile(new UpdateUserInput()
            {
                Nickname = nickname,
            });
            Assert.AreEqual(user.Nickname, nickname);
        }

        [Test]
        public async Task UpdatePassword()
        {
            var username = RandomString();
            var password = "123456";
            await client.RegisterByUsername(username, password);
            await client.UpdatePassword("111111", "123456");
        }

        [Test]
        public async Task UpdatePhone()
        {
            var username = RandomString();
            var password = "123456";
            await client.RegisterByUsername(username, password);
            await client.LoginByUsername(username, password);
            await client.UpdatePhone("17611161550", "1234");
        }

        [Test]
        public async Task UpdateEmail()
        {
            var email = RandomString() + "@gmail.com";
            var newEmail = RandomString() + "@gmail.com";
            var password = "123456";
            await client.RegisterByEmail(email, password);
            await client.LoginByEmail(email, password);
            await client.UpdateEmail(newEmail, "1234");
        }

        [Test]
        public async Task BindPhone()
        {
            var email = RandomString() + "@gmail.com";
            var password = "123456";
            await client.RegisterByEmail(email, password);
            await client.LoginByEmail(email, password);
            await client.BindPhone("17611161550", "1234");
        }

        [Test]
        public async Task UnbindPhone()
        {
            var email = RandomString() + "@gmail.com";
            var password = "123456";
            await client.RegisterByEmail(email, password);
            await client.LoginByEmail(email, password);
            await client.BindPhone("17611161550", "1234");
            await client.UnbindPhone();
        }

        [Test]
        public async Task RefreshToken()
        {
            var email = RandomString() + "@gmail.com";
            var password = "123456";
            await client.RegisterByEmail(email, password);
            await client.LoginByEmail(email, password);
            var refreshToken = await client.RefreshToken();
            Assert.AreNotEqual(refreshToken.Token, null);
        }

        [Test]
        public async Task Logout()
        {
            var email = RandomString() + "@gmail.com";
            var password = "123456";
            await client.RegisterByEmail(email, password);
            await client.LoginByEmail(email, password);
            await client.Logout();
        }
    }
}
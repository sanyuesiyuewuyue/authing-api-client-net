﻿using Authing.ApiClient.Results;
using Authing.ApiClient.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Authing.ApiClient
{
    /// <summary>
    /// Authing 认证客户端类
    /// </summary>
    public class AuthenticationClient : BaseClient
    {
        /// <summary>
        /// 通过用户池 ID 初始化
        /// </summary>
        /// <param name="userPoolId">用户池 ID，可以在控制台获取</param>
        public AuthenticationClient(string userPoolId)
        {
            UserPoolId = userPoolId ?? throw new ArgumentNullException(nameof (userPoolId));
        }

        private User CurrentUser
        {
            get
            {
                return currentUser;
            }
            set
            {
                currentUser = value;
                AccessToken = value?.Token;
            }
        }
        private User currentUser;

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <param name="accessToken">用户 access token</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> GetCurrentUser(
            string accessToken = null,
            CancellationToken cancellationToken = default)
        {
            var param = new UserParam();
            var res = await Request<UserResponse>(param.CreateRequest(), cancellationToken, accessToken);
            return res.Result;
        }

        /// <summary>
        /// 通过邮箱注册
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <param name="profile">用户资料</param>
        /// <param name="forceLogin">强制登录</param>
        /// <param name="generateToken">自动生成 token</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> RegisterByEmail(
            string email,
            string password, 
            RegisterProfile profile = null, 
            bool forceLogin = false, 
            bool generateToken = false, 
            CancellationToken cancellationToken = default)
        {
            var param = new RegisterByEmailParam()
            {
                Input = new RegisterByEmailInput()
                {
                    Email = email,
                    Password = Encrypt(password),
                    Profile = profile,
                    ForceLogin = forceLogin,
                    GenerateToken = generateToken,
                }
            };

            var res = await Request<RegisterByEmailResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 通过用户名注册
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="profile">用户资料</param>
        /// <param name="forceLogin">强制登录</param>
        /// <param name="generateToken">自动生成 token</param>
        /// <param name="cancellationToken"></param>
        /// <returns>User</returns>
        public async Task<User> RegisterByUsername(
            string username, 
            string password,
            RegisterProfile profile = null,
            bool forceLogin = false, 
            bool generateToken = false, 
            CancellationToken cancellationToken = default)
        {
            var param = new RegisterByUsernameParam()
            {
                Input = new RegisterByUsernameInput()
                {
                    Username = username,
                    Password = Encrypt(password),
                    Profile = profile,
                    ForceLogin = forceLogin,
                    GenerateToken = generateToken,
                }
            };

            var res = await Request<RegisterByUsernameResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 通过手机号注册
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="code">手机号验证码</param>
        /// <param name="password">密码</param>
        /// <param name="profile">用户资料</param>
        /// <param name="forceLogin">强制登录</param>
        /// <param name="generateToken">自动生成 token</param>
        /// <param name="cancellationToken"></param>
        /// <returns>User</returns>
        public async Task<User> RegisterByPhoneCode(
            string phone,
            string code,
            string password = null,
            RegisterProfile profile = null,
            bool forceLogin = false,
            bool generateToken = false,
            CancellationToken cancellationToken = default)
        {
            var param = new RegisterByPhoneCodeParam()
            {
                Input = new RegisterByPhoneCodeInput()
                {
                    Phone = phone,
                    Code = code,
                    Password = Encrypt(password),
                    Profile = profile,
                    ForceLogin = forceLogin,
                    GenerateToken = generateToken,
                }
            };

            var res = await Request<RegisterByPhoneCodeResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendSmsCode(string phone, CancellationToken cancellationToken = default)
        {
            var httpClient = CreateHttpClient();
            var url = $"{Host}/api/v2/sms/send";

            var message = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(
                    new 
                    {
                        phone
                    },
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }), Encoding.UTF8, "application/json")
            };
            var result = await httpClient.SendAsync(message, cancellationToken);

            if (!result.IsSuccessStatusCode)
            {
                throw new AuthingException(result.ReasonPhrase, (int)result.StatusCode);
            }

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<SendSmsCodeResponse>(content);

            if (response.Code != 200)
            {
                throw new AuthingException(response.Message, response.Code);
            }
        }

        /// <summary>
        /// 通过邮箱登录
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <param name="autoRegister">自动注册</param>
        /// <param name="captchaCode">人机验证码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> LoginByEmail(
            string email, 
            string password,
            bool autoRegister = false, 
            string captchaCode = null,
            CancellationToken cancellationToken = default)
        {
            var param = new LoginByEmailParam()
            {
                Input = new LoginByEmailInput()
                {
                    Email = email,
                    Password = Encrypt(password),
                    AutoRegister = autoRegister,
                    CaptchaCode = captchaCode,
                }
            };

            var res = await Request<LoginByEmailResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 通过用户名登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="autoRegister">自动注册</param>
        /// <param name="captchaCode">人机验证码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> LoginByUsername(
            string username,
            string password,
            bool autoRegister = false,
            string captchaCode = null,
            CancellationToken cancellationToken = default)
        {
            var param = new LoginByUsernameParam()
            {
                Input = new LoginByUsernameInput()
                {
                    Username = username,
                    Password = Encrypt(password),
                    AutoRegister = autoRegister,
                    CaptchaCode = captchaCode,
                }
            };

            var res = await Request<LoginByUsernameResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 通过手机号验证码登录
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="code">验证码</param>
        /// <param name="autoRegister">自动注册</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> LoginByPhoneCode(
            string phone,
            string code,
            bool autoRegister = false,
            CancellationToken cancellationToken = default)
        {
            var param = new LoginByPhoneCodeParam()
            {
                Input = new LoginByPhoneCodeInput()
                {
                    Phone = phone,
                    Code = code,
                    AutoRegister = autoRegister,
                }
            };

            var res = await Request<LoginByPhoneCodeResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 通过手机号密码登录
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="password">密码</param>
        /// <param name="autoRegister">自动注册</param>
        /// <param name="captchaCode">人机验证码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> LoginByPhonePassword(
            string phone,
            string password,
            bool autoRegister = false,
            string captchaCode = null,
            CancellationToken cancellationToken = default)
        {
            var param = new LoginByPhonePasswordParam()
            {
                Input = new LoginByPhonePasswordInput()
                {
                    Phone = phone,
                    Password = Encrypt(password),
                    AutoRegister = autoRegister,
                    CaptchaCode = captchaCode,
                }
            };

            var res = await Request<LoginByPhonePasswordResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 检查登录状态
        /// </summary>
        /// <param name="accessToken">用户的 access token</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<JWTTokenStatus> CheckLoginStatus(
            string accessToken = null,
            CancellationToken cancellationToken = default)
        {
            var param = new CheckLoginStatusParam();
            var res = await Request<CheckLoginStatusResponse>(param.CreateRequest(), cancellationToken, accessToken);
            return res.Result;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">邮件</param>
        /// <param name="scene"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommonMessage> SendEmail(
            string email,
            EmailScene scene,
            CancellationToken cancellationToken = default)
        {
            var param = new SendEmailParam()
            {
                Email = email,
                Scene = scene,
            };
            var res = await Request<SendEmailResponse>(param.CreateRequest(), cancellationToken);
            return res.Result;
        }

        /// <summary>
        /// 通过手机号验证码重置密码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="code">验证码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommonMessage> ResetPasswordByPhoneCode(
            string phone,
            string code,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            var param = new ResetPasswordParam()
            {
                Phone = phone,
                Code = code,
                NewPassword = Encrypt(newPassword),
            };
            var res = await Request<ResetPasswordResponse>(param.CreateRequest(), cancellationToken);
            return res.Result;
        }

        /// <summary>
        /// 通过邮箱验证码重置密码
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="code">验证码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommonMessage> ResetPasswordByEmailCode(
            string email,
            string code,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            var param = new ResetPasswordParam()
            {
                Email = email,
                Code = code,
                NewPassword = Encrypt(newPassword),
            };
            var res = await Request<ResetPasswordResponse>(param.CreateRequest(), cancellationToken);
            return res.Result;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="updates">更新项</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> UpdateProfile(
            UpdateUserInput updates,
            CancellationToken cancellationToken = default)
        {
            var param = new UpdateUserParam()
            {
                Input = updates,
            };
            var res = await Request<UpdateUserResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="newPassword">新密码</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> UpdatePassword(
            string newPassword,
            string oldPassword,
            CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new UpdatePasswordParam()
            {
                NewPassword = Encrypt(newPassword),
                OldPassword = Encrypt(oldPassword),
            };
            var res = await Request<UpdatePasswordResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 更新手机号
        /// </summary>
        /// <param name="phone">新手机号</param>
        /// <param name="phoneCode">新手机号的验证码</param>
        /// <param name="oldPhone">旧手机号</param>
        /// <param name="oldPhoneCode">旧手机号的验证码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> UpdatePhone(
            string phone,
            string phoneCode,
            string oldPhone = null,
            string oldPhoneCode = null,
            CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new UpdatePhoneParam()
            {
                Phone = phone,
                PhoneCode = phoneCode,
                OldPhone = oldPhone,
                OldPhoneCode = oldPhoneCode,
            };
            var res = await Request<UpdatePhoneResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 更新邮箱
        /// </summary>
        /// <param name="email">新邮箱</param>
        /// <param name="emailCode">新邮箱的验证码</param>
        /// <param name="oldEmail">旧邮箱</param>
        /// <param name="oldEmailCode">旧邮箱的验证码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> UpdateEmail(
            string email,
            string emailCode,
            string oldEmail = null,
            string oldEmailCode = null,
            CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new UpdateEmailParam()
            {
                Email = email,
                EmailCode = emailCode,
                OldEmail = oldEmail,
                OldEmailCode = oldEmailCode,
            };
            var res = await Request<UpdateEmailResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 绑定手机号，如果已绑定则会报错
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="phoneCode">手机号验证码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> BindPhone(
            string phone,
            string phoneCode,
            CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new BindPhoneParam()
            {
                Phone = phone,
                PhoneCode = phoneCode,
            };
            var res = await Request<BindPhoneResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 解绑定手机号，如果未绑定则会报错
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> UnbindPhone(CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new UnbindPhoneParam();
            var res = await Request<UnbindPhoneResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = res.Result;
            return res.Result;
        }

        /// <summary>
        /// 刷新 access token
        /// </summary>
        /// <param name="accessToken">用户 access token</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<RefreshToken> RefreshToken(
            string accessToken = null,
            CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new RefreshTokenParam();
            var res = await Request<RefreshTokenResponse>(param.CreateRequest(), cancellationToken, accessToken);
            return res.Result;
        }

        /// <summary>
        /// 获取用户自定义字段的值列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserDefinedData>> ListUdv(CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new UdvParam()
            {
                TargetId = CurrentUser.Id,
                TargetType = UdfTargetType.USER,
            };
            var res = await Request<UdvResponse>(param.CreateRequest(), cancellationToken);
            return res.Result;
        }

        /// <summary>
        /// 设置自定义字段值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserDefinedData>> AddUdv(
            string key,
            object value,
            CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new SetUdvParam()
            {
                Key = key,
                Value = JsonConvert.SerializeObject(value),
                TargetId = CurrentUser.Id,
                TargetType = UdfTargetType.USER,
            };
            var res = await Request<SetUdvResponse>(param.CreateRequest(), cancellationToken);
            return res.Result;
        }

        /// <summary>
        /// 移除用户自定义字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserDefinedData>> RemoveUdv(
            string key,
            CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new RemoveUdvParam()
            {
                Key = key,
                TargetId = CurrentUser.Id,
                TargetType = UdfTargetType.USER,
            };
            var res = await Request<RemoveUdvResponse>(param.CreateRequest(), cancellationToken);
            return res.Result;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Logout(CancellationToken cancellationToken = default)
        {
            await CheckLoggedIn();
            var param = new UpdateUserParam()
            {
                Id = CurrentUser.Id,
                Input = new UpdateUserInput()
                {
                    TokenExpiredAt = "0",
                }
            };
            await Request<UpdateUserResponse>(param.CreateRequest(), cancellationToken);
            CurrentUser = null;
        }

        private async Task CheckLoggedIn()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                throw new AuthingException("请先登录");
            }
        }
    }
}

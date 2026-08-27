using System.Net;
using System.Net.Mail;
using AADYA_HouseOfFragrance.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace AADYA_HouseOfFragrance.Services
{
    /// <summary>
    /// Service for sending email notifications including OTP, order confirmations,
    /// status updates, password alerts, and welcome emails.
    /// Uses SMTP configuration from appsettings.json.
    /// </summary>
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the EmailService.
        /// </summary>
        /// <param name="configuration">Configuration for SMTP settings.</param>
        /// <param name="httpContextAccessor">Optional accessor for building dynamic URLs.</param>
        public EmailService(IConfiguration configuration, IHttpContextAccessor? httpContextAccessor = null)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // ============================================
        // BASE EMAIL SENDING
        // ============================================

        /// <summary>
        /// Sends an email using the configured SMTP server.
        /// </summary>
        /// <param name="toEmail">Recipient email address.</param>
        /// <param name="subject">Email subject line.</param>
        /// <param name="body">HTML email body.</param>
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Smtp");
                var host = smtpSettings["Host"] ?? "smtp.gmail.com";
                var portStr = smtpSettings["Port"] ?? "587";
                int.TryParse(portStr, out int port);
                var username = smtpSettings["Username"] ?? string.Empty;
                var password = smtpSettings["Password"] ?? string.Empty;
                var fromEmail = smtpSettings["FromEmail"] ?? "noreply@aadya.in";

                using var smtpClient = new SmtpClient(host)
                {
                    Port = port > 0 ? port : 587,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    Timeout = 10000 // 10 second timeout
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "AADYA: The House of Fragrance"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log error but don't throw - email failures should not break the application flow
                Console.WriteLine($"[Email Error] Failed to send email to {toEmail}: {ex.Message}");
                throw; // Re-throw if caller wants to handle it
            }
        }

        // ============================================
        // HELPER: DYNAMIC BASE URL
        // ============================================

        /// <summary>
        /// Gets the base URL dynamically from the current HTTP request.
        /// Falls back to localhost if no request context is available.
        /// </summary>
        private string GetBaseUrl()
        {
            if (_httpContextAccessor?.HttpContext?.Request is HttpRequest request)
            {
                var scheme = request.Scheme;
                var host = request.Host.Value;
                return $"{scheme}://{host}";
            }

            // Fallback for background jobs or testing
            return "https://localhost:5000";
        }

        // ============================================
        // OTP EMAIL (Password Reset)
        // ============================================

        /// <summary>
        /// Generates the HTML body for OTP email.
        /// </summary>
        public string GetOtpEmailBody(string otp, string userName)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Montserrat', sans-serif; color: #222222; }}
                        .container {{ max-width: 500px; margin: 0 auto; padding: 2rem; background: #FAF6EF; border: 1px solid #D3C5B2; border-radius: 4px; }}
                        .header {{ text-align: center; margin-bottom: 1.5rem; }}
                        .header h1 {{ font-family: 'Playfair Display', serif; color: #3B0E14; margin: 0; }}
                        .subtitle {{ color: #666666; font-size: 0.85rem; }}
                        .otp-box {{ 
                            background: #FFFFFF; 
                            border: 2px dashed #CBA463; 
                            border-radius: 4px; 
                            padding: 1rem; 
                            text-align: center; 
                            font-size: 2rem; 
                            font-weight: 700; 
                            color: #3B0E14; 
                            letter-spacing: 4px; 
                            margin: 1rem 0; 
                        }}
                        .footer {{ margin-top: 1.5rem; padding-top: 1rem; border-top: 1px solid #D3C5B2; font-size: 0.75rem; color: #666666; text-align: center; }}
                        .expiry {{ color: #d32f2f; font-size: 0.8rem; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset OTP</h1>
                            <p class='subtitle'>AADYA: The House of Fragrance</p>
                        </div>
                        <p>Hello <strong>{userName}</strong>,</p>
                        <p>Use the OTP below to reset your password:</p>
                        <div class='otp-box'>{otp}</div>
                        <p class='expiry'>⏳ This OTP will expire in <strong>10 minutes</strong>.</p>
                        <p>If you didn't request this, please ignore this email.</p>
                        <div class='footer'>
                            &copy; 2026 AADYA: The House of Fragrance. All Rights Reserved.
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        // ============================================
        // ORDER CONFIRMATION
        // ============================================

        /// <summary>
        /// Sends an order confirmation email with all order details.
        /// </summary>
        public async Task SendOrderConfirmationAsync(string toEmail, string userName, Order order, List<OrderItem> items)
        {
            var subject = $"Order Confirmation #AAD-{order.OrderId:D4} - AADYA";
            var body = GetOrderConfirmationBody(order, items, userName);
            await SendEmailAsync(toEmail, subject, body);
        }

        private string GetOrderConfirmationBody(Order order, List<OrderItem> items, string userName)
        {
            var baseUrl = GetBaseUrl();

            var itemsHtml = string.Join("", items.Select(item =>
            {
                var productName = item.ProductVariant?.Product?.Name ?? "Unknown Product";
                var size = item.ProductVariant?.Size ?? "N/A";
                var unitPrice = item.UnitPriceAtSale;
                var total = unitPrice * item.Quantity;

                return $@"
                    <tr>
                        <td>{productName}</td>
                        <td>{size}</td>
                        <td>{item.Quantity}</td>
                        <td style='text-align:right;'>₹{unitPrice:F2}</td>
                        <td style='text-align:right;'>₹{total:F2}</td>
                    </tr>";
            }));

            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: 'Montserrat', sans-serif; color: #222; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 2rem; background: #FAF6EF; border: 1px solid #D3C5B2; border-radius: 4px; }}
                    .header {{ text-align: center; border-bottom: 2px solid #CBA463; padding-bottom: 1rem; margin-bottom: 1.5rem; }}
                    .header h1 {{ font-family: 'Playfair Display', serif; color: #3B0E14; margin: 0; }}
                    .order-id {{ font-size: 1.2rem; font-weight: 700; color: #3B0E14; }}
                    table {{ width: 100%; border-collapse: collapse; margin: 1rem 0; }}
                    th {{ background: #E8DFCF; padding: 0.6rem; text-align: left; font-size: 0.8rem; text-transform: uppercase; }}
                    td {{ padding: 0.6rem; border-bottom: 1px solid #D3C5B2; }}
                    .total {{ font-size: 1.2rem; font-weight: 700; text-align: right; }}
                    .footer {{ margin-top: 2rem; padding-top: 1rem; border-top: 1px solid #D3C5B2; font-size: 0.75rem; color: #666; text-align: center; }}
                    .btn {{ background: #3B0E14; color: #FAF6EF; padding: 0.4rem 1.2rem; text-decoration: none; border-radius: 2px; font-weight: 600; display: inline-block; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Order Confirmed</h1>
                        <p>AADYA: The House of Fragrance</p>
                    </div>
                    <p>Dear <strong>{userName}</strong>,</p>
                    <p>Thank you for your order! We are delighted to craft this experience for you.</p>
                    <p class='order-id'>Order ID: #AAD-{order.OrderId:D4}</p>
                    <p><strong>Shipping Address:</strong> {order.ShippingAddress}</p>
                    <p><strong>Order Status:</strong> {order.OrderStatus}</p>
                    <table>
                        <thead>
                            <tr>
                                <th>Product</th>
                                <th>Size</th>
                                <th>Qty</th>
                                <th style='text-align:right;'>Unit</th>
                                <th style='text-align:right;'>Total</th>
                            </tr>
                        </thead>
                        <tbody>{itemsHtml}</tbody>
                    </table>
                    <div class='total'>Grand Total: ₹{order.TotalAmount:F2}</div>
                    <p style='margin-top:1.5rem;'>Your order is being processed with care. You will receive updates as it ships.</p>
                    <p style='margin-top:0.5rem;'>
                        <a href='{baseUrl}/Account/Orders' class='btn'>View Your Orders</a>
                    </p>
                    <div class='footer'>&copy; 2026 AADYA: The House of Fragrance. All Rights Reserved.</div>
                </div>
            </body>
            </html>";
        }

        // ============================================
        // ORDER STATUS UPDATE
        // ============================================

        /// <summary>
        /// Sends an email notification when an order's status changes.
        /// Optionally includes a reason for cancellation or rejection.
        /// </summary>
        public async Task SendOrderStatusUpdateAsync(string toEmail, string userName, Order order, string? reason = null)
        {
            var subject = $"Order #AAD-{order.OrderId:D4} Status Update - AADYA";
            var body = GetOrderStatusUpdateBody(order, userName, reason);
            await SendEmailAsync(toEmail, subject, body);
        }

        private string GetOrderStatusUpdateBody(Order order, string userName, string? reason = null)
        {
            var baseUrl = GetBaseUrl();

            var statusColor = order.OrderStatus.ToLower() switch
            {
                "delivered" => "#2e7d32",
                "cancelled" => "#d32f2f",
                "dispatched" => "#0c5460",
                _ => "#856404"
            };

            var reasonHtml = string.IsNullOrEmpty(reason) ? string.Empty : $"<p><strong>Reason:</strong> {reason}</p>";

            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: 'Montserrat', sans-serif; color: #222; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 2rem; background: #FAF6EF; border: 1px solid #D3C5B2; border-radius: 4px; }}
                    .header {{ text-align: center; border-bottom: 2px solid #CBA463; padding-bottom: 1rem; margin-bottom: 1.5rem; }}
                    .header h1 {{ font-family: 'Playfair Display', serif; color: #3B0E14; margin: 0; }}
                    .status {{ font-size: 1.5rem; font-weight: 700; color: {statusColor}; }}
                    .footer {{ margin-top: 2rem; padding-top: 1rem; border-top: 1px solid #D3C5B2; font-size: 0.75rem; color: #666; text-align: center; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Order Update</h1>
                        <p>AADYA: The House of Fragrance</p>
                    </div>
                    <p>Dear <strong>{userName}</strong>,</p>
                    <p>Your order <strong>#AAD-{order.OrderId:D4}</strong> has been updated.</p>
                    <p class='status'>Status: {order.OrderStatus}</p>
                    {reasonHtml}
                    <p>To view full details, please visit your <a href='{baseUrl}/Account/Orders'>Order History</a>.</p>
                    <div class='footer'>&copy; 2026 AADYA: The House of Fragrance. All Rights Reserved.</div>
                </div>
            </body>
            </html>";
        }

        // ============================================
        // PASSWORD CHANGED ALERT
        // ============================================

        /// <summary>
        /// Sends a security alert email when a user's password is changed.
        /// </summary>
        public async Task SendPasswordChangedAlertAsync(string toEmail, string userName)
        {
            var subject = "Security Alert: Password Changed - AADYA";
            var body = GetPasswordChangedAlertBody(userName);
            await SendEmailAsync(toEmail, subject, body);
        }

        private string GetPasswordChangedAlertBody(string userName)
        {
            var baseUrl = GetBaseUrl();

            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: 'Montserrat', sans-serif; color: #222; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 2rem; background: #FAF6EF; border: 1px solid #D3C5B2; border-radius: 4px; }}
                    .header {{ text-align: center; border-bottom: 2px solid #CBA463; padding-bottom: 1rem; margin-bottom: 1.5rem; }}
                    .header h1 {{ font-family: 'Playfair Display', serif; color: #3B0E14; margin: 0; }}
                    .alert-box {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 1rem; margin: 1rem 0; }}
                    .footer {{ margin-top: 2rem; padding-top: 1rem; border-top: 1px solid #D3C5B2; font-size: 0.75rem; color: #666; text-align: center; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'><h1>Password Changed</h1></div>
                    <p>Dear <strong>{userName}</strong>,</p>
                    <div class='alert-box'><strong>⚠️ Security Alert:</strong> Your password was successfully changed.</div>
                    <p>If you did <strong>not</strong> initiate this change, please contact our support team immediately.</p>
                    <p>You can also <a href='{baseUrl}/Account/Login'>login here</a> to review your account.</p>
                    <div class='footer'>&copy; 2026 AADYA. All Rights Reserved.</div>
                </div>
            </body>
            </html>";
        }

        // ============================================
        // WELCOME EMAIL
        // ============================================

        /// <summary>
        /// Sends a welcome email to newly registered users.
        /// </summary>
        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Welcome to AADYA: The House of Fragrance";
            var body = GetWelcomeEmailBody(userName);
            await SendEmailAsync(toEmail, subject, body);
        }

        private string GetWelcomeEmailBody(string userName)
        {
            var baseUrl = GetBaseUrl();

            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: 'Montserrat', sans-serif; color: #222; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 2rem; background: #FAF6EF; border: 1px solid #D3C5B2; border-radius: 4px; }}
                    .header {{ text-align: center; border-bottom: 2px solid #CBA463; padding-bottom: 1rem; margin-bottom: 1.5rem; }}
                    .header h1 {{ font-family: 'Playfair Display', serif; color: #3B0E14; margin: 0; }}
                    .cta {{ text-align: center; margin: 1.5rem 0; }}
                    .btn {{ background: #3B0E14; color: #FAF6EF; padding: 0.6rem 1.5rem; text-decoration: none; border-radius: 2px; font-weight: 600; display: inline-block; }}
                    .footer {{ margin-top: 2rem; padding-top: 1rem; border-top: 1px solid #D3C5B2; font-size: 0.75rem; color: #666; text-align: center; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Welcome to AADYA</h1>
                        <p>The House of Fragrance</p>
                    </div>
                    <p>Dear <strong>{userName}</strong>,</p>
                    <p>Thank you for joining AADYA! You are now part of a community that celebrates pure, alcohol-free luxury.</p>
                    <p>Explore our curated collection and find your signature scent.</p>
                    <div class='cta'>
                        <a href='{baseUrl}/Home/Catalog' class='btn'>DISCOVER FRAGRANCES</a>
                    </div>
                    <div class='footer'>&copy; 2026 AADYA. All Rights Reserved.</div>
                </div>
            </body>
            </html>";
        }
    }
}
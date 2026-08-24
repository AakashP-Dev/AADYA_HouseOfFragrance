using System.Net;
using System.Net.Mail;
using AADYA_HouseOfFragrance.Models.Entities;

namespace AADYA_HouseOfFragrance.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ---- BASE SEND METHOD ----
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("Smtp");
            var host = smtpSettings["Host"] ?? "smtp.gmail.com";
            var portStr = smtpSettings["Port"] ?? "587";
            int.TryParse(portStr, out int port);
            var username = smtpSettings["Username"] ?? "";
            var password = smtpSettings["Password"] ?? "";
            var fromEmail = smtpSettings["FromEmail"] ?? "noreply@aadya.in";

            var smtpClient = new SmtpClient(host)
            {
                Port = port > 0 ? port : 587,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "AADYA: The House of Fragrance"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }

        // ---- OTP EMAIL (Existing) ----
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

        // ---- ORDER CONFIRMATION ----
        public async Task SendOrderConfirmationAsync(string toEmail, string userName, Order order, List<OrderItem> items)
        {
            var subject = $"Order Confirmation #AAD-{order.OrderId:D4} - AADYA";
            var body = GetOrderConfirmationBody(order, items, userName);
            await SendEmailAsync(toEmail, subject, body);
        }

        private string GetOrderConfirmationBody(Order order, List<OrderItem> items, string userName)
        {
            var itemsHtml = string.Join("", items.Select(item =>
                $"<tr><td>{item.ProductVariant?.Product?.Name ?? "Product"}</td><td>{item.ProductVariant?.Size ?? "N/A"}</td><td>{item.Quantity}</td><td style='text-align:right;'>₹{item.UnitPriceAtSale:F2}</td><td style='text-align:right;'>₹{(item.UnitPriceAtSale * item.Quantity):F2}</td></tr>"
            ));

            return $@"
            <html>
            <head><style>
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
            </style></head>
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
                        <thead><tr><th>Product</th><th>Size</th><th>Qty</th><th style='text-align:right;'>Unit</th><th style='text-align:right;'>Total</th></tr></thead>
                        <tbody>{itemsHtml}</tbody>
                    </table>
                    <div class='total'>Grand Total: ₹{order.TotalAmount:F2}</div>
                    <p style='margin-top:1.5rem;'>Your order is being processed with care. You will receive updates as it ships.</p>
                    <div class='footer'>&copy; 2026 AADYA: The House of Fragrance. All Rights Reserved.</div>
                </div>
            </body>
            </html>";
        }

        // ---- ORDER STATUS UPDATE ----
        public async Task SendOrderStatusUpdateAsync(string toEmail, string userName, Order order, string? reason = null)
        {
            var subject = $"Order #AAD-{order.OrderId:D4} Status Update - AADYA";
            var body = GetOrderStatusUpdateBody(order, userName, reason);
            await SendEmailAsync(toEmail, subject, body);
        }

        private string GetOrderStatusUpdateBody(Order order, string userName, string? reason = null)
        {
            var statusColor = order.OrderStatus.ToLower() switch
            {
                "delivered" => "#2e7d32",
                "cancelled" => "#d32f2f",
                "dispatched" => "#0c5460",
                _ => "#856404"
            };

            var reasonHtml = string.IsNullOrEmpty(reason) ? "" : $"<p><strong>Reason:</strong> {reason}</p>";

            return $@"
            <html>
            <head><style>
                body {{ font-family: 'Montserrat', sans-serif; color: #222; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 2rem; background: #FAF6EF; border: 1px solid #D3C5B2; border-radius: 4px; }}
                .header {{ text-align: center; border-bottom: 2px solid #CBA463; padding-bottom: 1rem; margin-bottom: 1.5rem; }}
                .header h1 {{ font-family: 'Playfair Display', serif; color: #3B0E14; margin: 0; }}
                .status {{ font-size: 1.5rem; font-weight: 700; color: {statusColor}; }}
                .footer {{ margin-top: 2rem; padding-top: 1rem; border-top: 1px solid #D3C5B2; font-size: 0.75rem; color: #666; text-align: center; }}
            </style></head>
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
                    <p>To view full details, please visit your <a href='https://yourdomain.com/Account/Orders'>Order History</a>.</p>
                    <div class='footer'>&copy; 2026 AADYA: The House of Fragrance. All Rights Reserved.</div>
                </div>
            </body>
            </html>";
        }

        // ---- PASSWORD CHANGED ALERT ----
        public async Task SendPasswordChangedAlertAsync(string toEmail, string userName)
        {
            var subject = "Security Alert: Password Changed - AADYA";
            var body = GetPasswordChangedAlertBody(userName);
            await SendEmailAsync(toEmail, subject, body);
        }

        private string GetPasswordChangedAlertBody(string userName)
        {
            return $@"
            <html>
            <head><style>
                body {{ font-family: 'Montserrat', sans-serif; color: #222; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 2rem; background: #FAF6EF; border: 1px solid #D3C5B2; border-radius: 4px; }}
                .header {{ text-align: center; border-bottom: 2px solid #CBA463; padding-bottom: 1rem; margin-bottom: 1.5rem; }}
                .header h1 {{ font-family: 'Playfair Display', serif; color: #3B0E14; margin: 0; }}
                .alert-box {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 1rem; margin: 1rem 0; }}
                .footer {{ margin-top: 2rem; padding-top: 1rem; border-top: 1px solid #D3C5B2; font-size: 0.75rem; color: #666; text-align: center; }}
            </style></head>
            <body>
                <div class='container'>
                    <div class='header'><h1>Password Changed</h1></div>
                    <p>Dear <strong>{userName}</strong>,</p>
                    <div class='alert-box'><strong>⚠️ Security Alert:</strong> Your password was successfully changed.</div>
                    <p>If you did <strong>not</strong> initiate this change, please contact our support team immediately.</p>
                    <div class='footer'>&copy; 2026 AADYA. All Rights Reserved.</div>
                </div>
            </body>
            </html>";
        }

        // ---- WELCOME EMAIL ----
        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Welcome to AADYA: The House of Fragrance";
            var body = GetWelcomeEmailBody(userName);
            await SendEmailAsync(toEmail, subject, body);
        }

        private string GetWelcomeEmailBody(string userName)
        {
            return $@"
            <html>
            <head><style>
                body {{ font-family: 'Montserrat', sans-serif; color: #222; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 2rem; background: #FAF6EF; border: 1px solid #D3C5B2; border-radius: 4px; }}
                .header {{ text-align: center; border-bottom: 2px solid #CBA463; padding-bottom: 1rem; margin-bottom: 1.5rem; }}
                .header h1 {{ font-family: 'Playfair Display', serif; color: #3B0E14; margin: 0; }}
                .cta {{ text-align: center; margin: 1.5rem 0; }}
                .btn {{ background: #3B0E14; color: #FAF6EF; padding: 0.6rem 1.5rem; text-decoration: none; border-radius: 2px; font-weight: 600; }}
                .footer {{ margin-top: 2rem; padding-top: 1rem; border-top: 1px solid #D3C5B2; font-size: 0.75rem; color: #666; text-align: center; }}
            </style></head>
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
                        <a href='https://yourdomain.com/Home/Catalog' class='btn'>DISCOVER FRAGRANCES</a>
                    </div>
                    <div class='footer'>&copy; 2026 AADYA. All Rights Reserved.</div>
                </div>
            </body>
            </html>";
        }
    }
}
using Quanta.Forms.Models;

namespace Quanta.Forms.Notifications;

internal static class EmailTemplate
{
    public static string BuildHtml(FormSubmission s, DateTimeOffset ts) => $"""
        <!DOCTYPE html>
        <html>
        <body style="font-family:'Helvetica Neue',Arial,sans-serif;max-width:560px;margin:0 auto;color:#1A1612;">
          <div style="background:#0B1F3A;padding:24px 32px;">
            <span style="font-size:20px;font-weight:700;letter-spacing:0.1em;color:#fff;">QUANTA</span>
            <div style="height:2px;width:80px;background:#C4922A;margin:4px 0 2px;"></div>
            <span style="font-size:9px;letter-spacing:0.4em;color:rgba(255,255,255,0.4);text-transform:uppercase;">Capital Holdings</span>
          </div>
          <div style="padding:28px 32px;background:#F7F5F0;border:1px solid #E8E4DC;">
            <p style="font-size:13px;color:#8A8478;margin:0 0 20px;">New enquiry received · {ts:ddd, MMM d yyyy 'at' h:mm tt} UTC</p>
            <table style="width:100%;border-collapse:collapse;">
              <tr><td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;width:140px;">Form</td>
                  <td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;font-family:monospace;">{s.FormId}</td></tr>
              <tr><td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;width:140px;">Name</td>
                  <td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;">{s.FirstName} {s.LastName}</td></tr>
              <tr><td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;">Email</td>
                  <td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;"><a href="mailto:{s.Email}" style="color:#0B1F3A;">{s.Email}</a></td></tr>
              <tr><td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;">Phone</td>
                  <td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;"><a href="tel:{s.Phone}" style="color:#0B1F3A;">{s.Phone}</a></td></tr>
              {(string.IsNullOrWhiteSpace(s.Company) ? "" : $"<tr><td style=\"padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;\">Company</td><td style=\"padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;\">{s.Company}</td></tr>")}
              {(string.IsNullOrWhiteSpace(s.Industry) ? "" : $"<tr><td style=\"padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;\">Industry</td><td style=\"padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;\">{s.Industry}</td></tr>")}
            </table>
            {(string.IsNullOrWhiteSpace(s.Message) ? "" : $"<div style=\"margin-top:20px;padding:16px;background:white;border:1px solid #E8E4DC;border-radius:4px;\"><p style=\"font-size:12px;font-weight:600;color:#8A8478;margin:0 0 8px;letter-spacing:0.06em;text-transform:uppercase;\">Message</p><p style=\"font-size:14px;line-height:1.7;margin:0;\">{s.Message}</p></div>")}
            <div style="margin-top:24px;text-align:center;">
              <a href="mailto:{s.Email}?subject=Re: Your enquiry to Quanta Capital Holdings" style="display:inline-block;background:#C4922A;color:white;padding:12px 28px;border-radius:4px;font-size:14px;font-weight:600;text-decoration:none;">Reply to {s.FirstName}</a>
            </div>
          </div>
          <div style="padding:16px 32px;background:#071529;text-align:center;">
            <p style="font-size:11px;color:rgba(255,255,255,0.3);margin:0;">Quanta Capital Holdings Inc. · Toronto, Ontario, Canada</p>
          </div>
        </body>
        </html>
        """;
}

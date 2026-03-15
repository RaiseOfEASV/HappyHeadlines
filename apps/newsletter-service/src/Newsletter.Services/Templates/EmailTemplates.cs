namespace Newsletter.Services.Templates;

public static class EmailTemplates
{
    public static string BreakingNewsTemplate(string articleTitle, string articleContent, string continent)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #ff6b6b; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }}
        .article-title {{ font-size: 24px; font-weight: bold; margin-bottom: 15px; color: #ff6b6b; }}
        .article-content {{ margin-bottom: 20px; }}
        .metadata {{ font-size: 12px; color: #666; margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #999; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>BREAKING NEWS</h1>
        <p>HappyHeadlines Alert</p>
    </div>
    <div class='content'>
        <div class='article-title'>{articleTitle}</div>
        <div class='article-content'>{articleContent}</div>
        <div class='metadata'>
            <strong>Region:</strong> {continent}<br>
            <strong>Published:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC
        </div>
    </div>
    <div class='footer'>
        <p>You are receiving this because you subscribed to HappyHeadlines breaking news alerts.</p>
        <p><a href='{{unsubscribe_link}}'>Unsubscribe</a></p>
    </div>
</body>
</html>";
    }

    public static string DailyDigestTemplate(List<(string Title, string Content, string Continent)> articles)
    {
        var articlesHtml = string.Join("", articles.Select(a => $@"
        <div style='margin-bottom: 30px; padding-bottom: 20px; border-bottom: 1px solid #eee;'>
            <h3 style='color: #4ecdc4; margin-bottom: 10px;'>{a.Title}</h3>
            <p style='margin-bottom: 10px;'>{a.Content}</p>
            <p style='font-size: 12px; color: #666;'><strong>Region:</strong> {a.Continent}</p>
        </div>"));

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4ecdc4; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #999; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Daily Digest</h1>
        <p>Your Daily News Summary from HappyHeadlines</p>
        <p style='font-size: 14px;'>{DateTime.UtcNow:MMMM dd, yyyy}</p>
    </div>
    <div class='content'>
        <p>Here are today's top stories:</p>
        {articlesHtml}
    </div>
    <div class='footer'>
        <p>You are receiving this daily digest because you subscribed to HappyHeadlines.</p>
        <p><a href='{{unsubscribe_link}}'>Unsubscribe</a></p>
    </div>
</body>
</html>";
    }
}

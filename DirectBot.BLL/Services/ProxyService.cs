namespace DirectBot.BLL.Services;

public class ProxyService
{
    private readonly ApplicationDbContext _db;

    public ProxyService(ApplicationDbContext db)
    {
        _db = db;
    }

    public SelectList GetCountries()
    {
        return new SelectList(_db.Proxies.Select(proxy => proxy.Country).Distinct().ToList());
    }

    public void SetProxy(Instagram instagram)
    {
        var proxy = _db.Proxies.FromSqlRaw("SELECT * FROM instaq.proxies ORDER BY rand()")
            .FirstOrDefault(proxy1 => proxy1.Country == instagram.Country);
        if (proxy == null) return;
        instagram.Proxy = proxy;
        _db.SaveChangesAsync();
    }

    public bool DeleteProxy(Proxy proxy)
    {
        try
        {
            _db.Proxies.Remove(proxy);
            _db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool AddProxy(string proxies)
    {
        string[] proxyArray = proxies.Split('\n');
        foreach (var proxyString in proxyArray)
        {
            var data = proxyString.Split(":", 5);
            if (data.Length != 5) return false;
            if (!Org.BouncyCastle.Utilities.Net.IPAddress.IsValid(data[0])) return false;
            if (!int.TryParse(data[1], out int port)) return false;
            _db.Add(new Proxy()
                { Host = data[0], Port = port, Login = data[2], Password = data[3], Country = data[4].ToUpper()});
        }

        _db.SaveChanges();
        return true;
    }
}
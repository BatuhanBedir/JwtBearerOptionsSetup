using Jwt.Models;
using Jwt.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jwt.Providers;

public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly JwtOptions _jwtOptions;
    public JwtTokenProvider(IOptions<JwtOptions>options)    //optionslara git, JWtOptions'a karşılık gelen değerleri al.development.jsondaki veriler.
    //IOptions(singleton gibi çalışır. program ayağa kalktıgında verileri okur. program süresince verileri tek sefer içersinde okunur. Değişse bile okumaz),
    //IOptionsMonitor(Transiet)->her ihtiyacı oldugunda okur.
    //IOptionsSnapshot(scope)-> istek oldugu sürece okur.
    {
        _jwtOptions = options.Value;   
    }
    public string GenerateToken(AppUser user)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier,user.Id),
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Email,user.Email),
            new (JwtRegisteredClaimNames.GivenName, user.FirstName),
            new (JwtRegisteredClaimNames.FamilyName, user.LastName),
            new (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };  //tokenini içerisine atılacak veriler belirlendi. Token oluşturulurken bunu bir key kullanılarak(metin, harf) onunla beraber şifrelenmesini isteriz. 

        var encodedKey = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
        var signInCredentials = new SigningCredentials(new SymmetricSecurityKey(encodedKey),SecurityAlgorithms.HmacSha256);//giriş için credentials olusturduk
        var token = new JwtSecurityToken(issuer: _jwtOptions.Issuer, audience: _jwtOptions.Audience, claims: claims, expires: DateTime.Now.Add(_jwtOptions.ExpiredTime),signingCredentials:signInCredentials);//tokenın gövdesini olusturmuş olduk.

       return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

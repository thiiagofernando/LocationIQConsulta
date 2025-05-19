
using LocationIQConsulta;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;


const string LOCATIONIQ_APIKEY = "sua_chave";


static async Task<LocationIQResult> GeocodificarEnderecoLocationIQAsync(string enderecoCompleto)
{
    using var client = new HttpClient();
    string url = $"https://api.locationiq.com/v1/search?key={LOCATIONIQ_APIKEY}&q={Uri.EscapeDataString(enderecoCompleto)}&format=json&countrycodes=br&limit=1";

    try
    {
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        string json = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<LocationIQResult[]>(json);

        if (results != null && results.Length > 0)
            return results[0];
        else
            return null;
    }
    catch
    {
        return null;
    }
}

static async Task<ViaCepResponse> ObterEnderecoViaCepAsync(string cep)
{
    using var client = new HttpClient();
    string url = $"https://viacep.com.br/ws/{cep}/json/";

    try
    {
        var resposta = await client.GetAsync(url);
        if (!resposta.IsSuccessStatusCode)
            return null;

        string conteudo = await resposta.Content.ReadAsStringAsync();
        var endereco = JsonSerializer.Deserialize<ViaCepResponse>(conteudo);

        if (endereco == null || !string.IsNullOrEmpty(endereco.erro))
            return null;

        return endereco;
    }
    catch
    {
        return null;
    }
}



var cep = "78150322"; //cep para pequisar

var endereco = await ObterEnderecoViaCepAsync(cep);

if (endereco == null)
{
    Console.WriteLine("Não foi possível obter o endereço via CEP.");
    return;
}

var enderecoCompleto = $"{endereco.logradouro}, {endereco.bairro}, {endereco.localidade}, {endereco.uf}, Brasil";
Console.WriteLine($"\nEndereço encontrado: {enderecoCompleto}");

var localizacao = await GeocodificarEnderecoLocationIQAsync(enderecoCompleto);

if (localizacao != null)
{
    Console.WriteLine($"\nDescrição no LocationIQ: {localizacao.display_name}");
    Console.WriteLine($"Latitude: {localizacao.lat}");
    Console.WriteLine($"Longitude: {localizacao.lon}");
}
else
{
    Console.WriteLine("Não foi possível obter coordenadas no LocationIQ.");
}
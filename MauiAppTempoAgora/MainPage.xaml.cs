using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage() => InitializeComponent();

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsão = "";

                        dados_previsão = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Temp Máx: {t.temp_max} \n" +
                                         $"Temp Min: {t.temp_min} \n";


                        lbl_res.Text = dados_previsão;

                        string mapa = $"https://embed.windy.com/embed.html?" +
                                      $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                                      $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                      $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",", ".")}";

                        mv_mapa.Source = mapa;
                        Debug.WriteLine(mapa);


                    } else
                    {
                        lbl_res.Text = "Sem dados de Previsão";
                    }
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }


        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(
                        GeolocationAccuracy.Medium,
                        TimeSpan.FromSeconds(10)
                        );

                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if (local != null)
                {
                    lbl_coords.Text = $"Latitude: {local.Latitude} \n" +
                                        $"Longitude: {local.Longitude}";

                    lbl_coords.Text = local_disp;

                    //pega nome da cidade que está nas coordenadas
                    await GetCidade(local.Latitude, local.Longitude);

                  


                    lbl_coords.Text += $"\nCidade: {txt_cidade.Text}";

                } else
                {
                    lbl_coords.Text = "Nenhuma localização";
                }

            } catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não Suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneex)
            {
                await DisplayAlert("Erro: Localização Desabilitada", fneex.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão da Localização", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }

        }
        private async Task GetCidade(double lat, double lon)
        {
            try
            {


                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);

                
                Placemark? place = places.FirstOrDefault();                

                if (place != null)
                {
                    string cidade = place.Locality ?? place.SubAdminArea ?? place.AdminArea ?? place.CountryName ?? "Cidade não encontrada";
                    txt_cidade.Text = cidade;
                    Debug.WriteLine($"Cidade resolvida: {cidade}");
                }
                else
                {
                    txt_cidade.Text = "Cidade não encontrada";
                    Debug.WriteLine("Nenhum placemark encontrado.");

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da Cidade", ex.Message, "OK");
            }
        }
    }
    

}



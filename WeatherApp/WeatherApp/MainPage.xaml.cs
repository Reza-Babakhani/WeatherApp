using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WeatherApp
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		private const string App_Id = "a1c0a5e5a15b72bc423021436303aca3";
		private HttpClient client;
		public MyWeather weather1;
		public string unit = "metric";


		public MainPage()
		{
			InitializeComponent();

			client = new HttpClient();

			client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");

			MakeVisible(false);
			loading.IsVisible = false;

		}


		private async Task GetWeather()
		{
			try
			{
				if (txtCity.Text.Trim() != "")
				{
					loading.IsVisible = true;
					var response = await client.GetAsync($"weather?q={txtCity.Text}&appid={App_Id}&units={unit}");
					string s = await response.Content.ReadAsStringAsync();
					if (response.StatusCode == HttpStatusCode.OK)
					{


						weather1 = new MyWeather();

						weather1 = JsonConvert.DeserializeObject<MyWeather>(s);

						BindingContext = weather1;

						DateTime dtSunRise = new DateTime(0);
						DateTime dtSunSet = new DateTime(0);


						txtSunrise.Text = dtSunRise.AddSeconds(weather1.sys.sunrise).AddSeconds(weather1.timezone).ToString("HH:mm");
						txtSunset.Text = dtSunSet.AddSeconds(weather1.sys.sunset).AddSeconds(weather1.timezone).ToString("HH:mm");
						loading.IsVisible = false;

						MakeVisible(true);

						Device.StartTimer(new TimeSpan(0, 0, 1), () =>
						{
							try
							{
								// do something every 60 seconds
								Device.BeginInvokeOnMainThread(() =>
							{
								try
								{
									txtClock.Text = DateTime.UtcNow.AddSeconds(weather1.timezone).ToString("HH:mm:ss");
								}
								catch
								{
									MakeVisible(false);
									weather1 = null;
									loading.IsVisible = false;


								}
								// interact with UI elements
							});
								return true; // runs again, or false to stop
							}
							catch
							{
								MakeVisible(false);
								weather1 = null;
								loading.IsVisible = false;

								return false;

							}
						});


					}
					else
					{
						MakeVisible(false);
						weather1 = null;
						loading.IsVisible = false;

						await DisplayAlert("Error", $"Response Error: {response.StatusCode} {s}", "Ok");

					}
				}
				else
				{
					MakeVisible(false);
					loading.IsVisible = false;

					weather1 = null;
					txtCity.Focus();
				}
			}
			catch (Exception ex)
			{
				weather1 = null;
				await DisplayAlert("Error", ex.Message, "Ok");
				txtCity.Text = "";
				loading.IsVisible = false;

				txtCity.Focus();
			}
		}



		private async void searchClick(object sender, EventArgs e)
		{

			await GetWeather();
		}

		private async void txtCity_Completed(object sender, EventArgs e)
		{
			await GetWeather();

		}

		private void btnMap_Clicked(object sender, EventArgs e)
		{
			if (weather1 != null)
			{
				try
				{
					Launcher.OpenAsync($"geo:{weather1.coord.lat},{weather1.coord.lon}");
				}
				catch
				{
					DisplayAlert("Error", "Map Not Found!", "Ok");
				}
			}
		}

		private void MakeVisible(bool visible)
		{
			grid1.IsVisible = visible;
			grid2.IsVisible = visible;
			grid3.IsVisible = visible;
			grid4.IsVisible = visible;
			grid5.IsVisible = visible;
			btnMap.IsVisible = visible;

		}
	}
}

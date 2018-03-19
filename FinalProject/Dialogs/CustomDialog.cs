using FinalProject.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FinalProject.Dialogs
{
    [Serializable]
    [LuisModel("adb2162d-3880-46d9-8cea-fad1c46cf924", "073cdcc519ef443db5e5e7a2a6f92a6b")]
    public class CustomDialog : LuisDialog<object>
    {

        [LuisIntent("Cumprimento")]
        public async Task Cumprimento(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Olá, em que posso lhe ajudar?");
        }

        [LuisIntent("Sobre")]
        public async Task Sobre(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sou o seu assistente, em que posso te ajudar?");
        }

        [LuisIntent("Vender")]
        public async Task Vender(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Carros disponíveis para venda:");
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = createHeroCardVender();

            await context.PostAsync(reply);
        }

        [LuisIntent("Alugar")]
        public async Task Alugar(IDialogContext context, LuisResult result)
        {
            double dolar = 0;

            using (var client = new HttpClient())
            {
                string url = "http://cotacaoapiconcessionaria.azurewebsites.net/api/cotacao";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    await context.PostAsync("Ocorreu um erro... Tente mais tarde");
                    return;
                }
                else
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<Cotacao[]>(json);
                    dolar = resultado[0].Valor;

                    await context.PostAsync($"Valor do aluguel baseado na cotação atual do dolar: ${dolar}");
                }
            }

            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = createHeroCardAlugar(dolar);

            await context.PostAsync(reply);
        }

        [LuisIntent("Localizacao")]
        public async Task Localizacao(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Estamos localizados no endereço abaixo:");

            var response = context.MakeMessage();
            response.Attachments.Add(CreateHeroCardLocalizacao());

            await context.PostAsync(response);
            
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("None");
        }
 
        //HERO CARD LOCAÇÃO
        public List<Attachment> createHeroCardAlugar(double dolar)
        {
            List<Attachment> list = new List<Attachment>();

            for (int i = 0; i < 2; i++)
            {

                var heroCard = new HeroCard();
                heroCard.Title = "Uno 1.0";
                heroCard.Subtitle = $"Valor: U$40/R${dolar*40} (dia)";
                heroCard.Text = "";
                heroCard.Images = new List<CardImage>()
                {
                    new CardImage(url: "https://yata.ostr.locaweb.com.br/f5648e8bcc978ba2ca8818707fcfa101c7f408140d326f3467939807d11e8ae6" )
                };
                    heroCard.Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.OpenUrl, "Alugar", value: "http://www.microsoft.com")
                };
                list.Add(heroCard.ToAttachment());
            }
            return list;
        }

        //HERO CARD VENDA
        public List<Attachment> createHeroCardVender()
        {
            List<Attachment> list = new List<Attachment>();

            for (int i = 0; i < 4; i++)
            {

                var heroCard = new HeroCard();
                heroCard.Title = "Bmw X";
                heroCard.Subtitle = "R$50.000";
                heroCard.Text = "Completo";
                heroCard.Images = new List<CardImage>()
                {
                    new CardImage(url: "http://pngimg.com/uploads/bmw/bmw_PNG1702.png" )
                };
                heroCard.Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.OpenUrl, "Comprar", value: "http://www.microsoft.com")
                };
                list.Add(heroCard.ToAttachment());
            }
            return list;
        }

        //HERO CARD LOCALIZAÇÃO
        public Attachment CreateHeroCardLocalizacao()
        {
            var heroCard = new HeroCard();
            heroCard.Title = "LOCBOT";
            heroCard.Subtitle = "Av. Alagados, Nº 123 - Santa Maria / Brasília-DF";
            //heroCard.Text = "Completo";
            heroCard.Images = new List<CardImage>()
            {
                new CardImage(url: "https://maps.googleapis.com/maps/api/staticmap?center=-16.049084,-48.030455&zoom=15&size=400x400&markers=color:blue%7Clabel:LOCBOT%7C-16.049084,-48.030455&format=png&key=AIzaSyAUc6UNm-9LJNO1G8Phl3sNwpocknGVUms" )
            };
            heroCard.Buttons = new List<CardAction>()
            {
                new CardAction(ActionTypes.OpenUrl, "Abrir no Mapa", value: "https://goo.gl/maps/Mb1XnQv8uJC2")
            };
            return heroCard.ToAttachment();
        }
    }
}
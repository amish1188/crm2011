using ExternalServices.PostfordelingService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace KlpCrm.Filenet.Web.ReactApplication.BusinessLogic
{
    public class GetAvailableKasseNrLogic
    {
        //this is old txtDokBeskrivelse_OnTextChanged
        public static ListDictionary setKasseNumbersValues(string forsikringstakerNummer, string department, string description)
        {
            

            using (PostfordelingServiceClient client = new PostfordelingServiceClient())
            {
                HentSaksbehandlereForForsikringstakerRequest request = new HentSaksbehandlereForForsikringstakerRequest();


                //request.forsikringsgivernummer = "1000";
                request.forsikringsgivernummer = department;
                request.forsikringstakernummer = forsikringstakerNummer.Trim();
                request.dokumentkode = SendToArchive.GetSelectedDokBeskrivelseKode(description);

                HentSaksbehandlereForForsikringstakerResponse response = client.hentSaksbehandlereForForsikringstaker(request);

                ListDictionary listItems = new ListDictionary();

                if (response != null)
                {
                    if (response.postfordelingsinformasjon != null)
                    {
                        Postfordelingsinformasjon postfordelingsinformasjon = response.postfordelingsinformasjon;
                        Kunde kunde = null;

                        if (postfordelingsinformasjon.kunde != null)
                            kunde = postfordelingsinformasjon.kunde;
                        if (postfordelingsinformasjon.saksbehandlere != null)
                        {

                            foreach (Saksbehandler saksbehandler in postfordelingsinformasjon.saksbehandlere)
                            {
                                if (saksbehandler.risikofellesskap == "00")
                                    listItems.Add("KS00 Gruppeliv/Læregruppeliv", "00");

                                if (saksbehandler.risikofellesskap == "10")
                                    listItems.Add("KS10 Felles pensjonsordning for fylkeskommuner", "10");

                                if (saksbehandler.risikofellesskap == "13")
                                    listItems.Add("KS13 Pensjonsordning for sykepleiere", "13");

                                if (saksbehandler.risikofellesskap == "15")
                                    listItems.Add("KS15 Pensjonsordning for sykehusleger", "15");

                                if (saksbehandler.risikofellesskap == "16")
                                    listItems.Add("KS16 Felles pensjonsordning for helseforetak", "16");

                                if (saksbehandler.risikofellesskap == "18")
                                    listItems.Add("KS18 Felles pensjonsordning for kommuner og bedrifter", "18");

                                if (saksbehandler.risikofellesskap == "23")
                                    listItems.Add("KS23 Pensjonsordning for folkevalgte", "23");

                                if (saksbehandler.risikofellesskap == "25")
                                    listItems.Add("KS25 Foretakspensjon", "25");

                                if (saksbehandler.risikofellesskap == "28")
                                    listItems.Add("KS28 Innskuddspensjon", "28");

                                if (saksbehandler.risikofellesskap == "30")
                                    listItems.Add("Lukket ordning", "30");
                            }
                        }

                    }
                }
                return listItems;
            }
        }
    }
}
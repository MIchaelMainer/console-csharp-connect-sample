﻿//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.
extern alias GraphBetaModels;
using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace console_csharp_connect_sample
{
    class Program
    {
        public static GraphServiceClient graphClient;
        
        static void Main(string[] args)
        {
            Console.SetWindowSize(150, 80);
            Console.WriteLine("Welcome to the C# Console Connect Sample!\n");

            try
            {
                //*********************************************************************
                // setup Microsoft Graph Client for user.
                //*********************************************************************
                if (Constants.ClientId != "ENTER_YOUR_CLIENT_ID")
                {
                    graphClient = AuthenticationHelper.GetAuthenticatedClient();
                    if (graphClient != null)
                    {

                        var user = graphClient.Me.Request().GetAsync().Result;
                        string userId = user.Id;
                        string mailAddress = user.UserPrincipalName;
                        string displayName = user.DisplayName;

                        Console.WriteLine("Hello, " + displayName + ". Would you like to get your trending information?");
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();
                        Console.WriteLine();
                    

                        string requestUrl = "https://graph.microsoft.com/beta/me/insights/trending";

                        HttpRequestMessage hrm = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                        // Authenticate (add access token) our HttpRequestMessage
                        graphClient.AuthenticationProvider.AuthenticateRequestAsync(hrm).GetAwaiter().GetResult();

                        // Send the request and get the response.
                        HttpResponseMessage response = graphClient.HttpProvider.SendAsync(hrm).Result;

                        // Get the trending object
                        if (response.IsSuccessStatusCode)
                        {
                            // Get the trending response.
                            var content = response.Content.ReadAsStringAsync().Result;

                            // Deserialize the trending response into a collection page.
                            // This is failing. It'd be ideal to get this working then we don't have to do any JSON manipulation.
                            // Getting your trending information failed with the following message: Cannot populate JSON object onto type 'Microsoft.Graph.OfficeGraphInsightsTrendingCollectionPage'. Path '@odata.context', line 2, position 20.
                            GraphBetaModels.Microsoft
                                           .Graph
                                           .GraphServiceTrendingCollectionPage trendings = graphClient.HttpProvider
                                                                                                            .Serializer
                                                                                                            .DeserializeObject<GraphBetaModels.Microsoft
                                                                                                                                              .Graph
                                                                                                                                              .GraphServiceTrendingCollectionPage>(content);

                            foreach (GraphBetaModels.Microsoft.Graph.Trending trendingItem in trendings)
                            {
                                Console.WriteLine($"Trending id: {trendingItem.Id}");
                                Console.WriteLine($"Trending resource title: {trendingItem.ResourceVisualization.Title}");
                                Console.WriteLine($"Trending resource preview text: {trendingItem.ResourceVisualization.PreviewText}");
                                Console.WriteLine($"Trending resource web url: {trendingItem.ResourceReference.WebUrl}\n");
                            }
                        }
                        Console.WriteLine("\n\nPress any key to continue.");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("We weren't able to create a GraphServiceClient for you. Please check the output for errors.");
                        Console.ResetColor();
                        Console.ReadKey();
                        return;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You haven't configured a value for ClientId in Constants.cs. Please follow the Readme instructions for configuring this application.");
                    Console.ResetColor();
                    Console.ReadKey();
                    return;
                }


            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Getting your trending information failed with the following message: {0}", ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Error detail: {0}", ex.InnerException.Message);
                }
                Console.ResetColor();
                Console.ReadKey();
                return;
            }



        }




    }


}


using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace Practice.API
{
    internal class ApiService
    {
        // ----------------------------------------------- Fields -------------------------------------------------------
        // A field that stores the path that is taken as a basis in the request.
        private string _baseUrl;


        // --------------------------------------------- Constructors ---------------------------------------------------
        // A single constructor with a base url parameter.
        public ApiService(string baseUrl)                                                                      
        {
            _baseUrl = ValidateURL(baseUrl);                                     
        }

        // --------------------------------------------- Properties -----------------------------------------------------
        // A property that allows you to get the private field of the base url and replace the value with another one.
        public string BaseUrl
        {
            get => _baseUrl;
            set => _baseUrl = ValidateURL(value);
        }

        // ---------------------------------------------- Methods -------------------------------------------------------
        // The method responsible for the get request, its structure and settings.
        public void SendGetRequest(params string[] prameters)
        {

        }

        // A method that checks the correctness of the URL.
        private string ValidateURL(string url)
        {
            if (!Regex.Match(url, "^http(s)?://([\\w-]+.)+[\\w-]+(/[\\w- ./?%&=])?$").Success)
            {
                MessageBox.Show("You have provided a base URL in an incorrect format", "Provision of data", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return url;
        }
    }
}
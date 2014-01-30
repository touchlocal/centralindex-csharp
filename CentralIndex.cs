using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Collections;
  

// CentralIndex class
public class CentralIndex
{
    public string apiKey;
    public bool debugMode;
    
    public CentralIndex(string key, bool mode) {
        apiKey = key;
        debugMode = mode;
        
    }
    
    public string HttpGet(string URI) {
       if(debugMode) {
         Console.WriteLine(URI);
       }
       System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
    //   req.Proxy = new System.Net.WebProxy(ProxyString, true); //true means no proxy
       System.Net.WebResponse resp = req.GetResponse();
       System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
       return sr.ReadToEnd().Trim();
    }
    
    public string HttpPost( String method, string URI,string Parameters) 
    {
       System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
     //  req.Proxy = new System.Net.WebProxy(ProxyString, true);
       //Add these, as we're doing a POST
       req.ContentType = "application/x-www-form-urlencoded";
       req.Method = method;
       //We need to count how many bytes we're sending. Post'ed Faked Forms should be name=value&
       byte [] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
       req.ContentLength = bytes.Length;
       System.IO.Stream os = req.GetRequestStream ();
       os.Write (bytes, 0, bytes.Length); //Push it out there
       os.Close ();
       System.Net.WebResponse resp = req.GetResponse();
       if (resp== null) return null;
       System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
       return sr.ReadToEnd().Trim();
    }
    
    public String doCurl(String method, String path, Hashtable p) {
         
        
         String url = "http://api.centralindex.com/v1"+path;
         String data = "";
        p.Add("api_key",apiKey);
        foreach( DictionaryEntry de in p )
        {
            String key = HttpUtility.UrlEncode((String) de.Key);
            String val = HttpUtility.UrlEncode((String) de.Value);
            data += key + "=" + val;
            data += "&";
        }

        // Create a request using a URL that can receive a post. 
        if(method=="GET") {
          url += "?"+data;
          return HttpGet(url);  
        } else {
          return HttpPost(method,url,data);   
        }

    }
    
  /**
   * Get the activity from the collection
   *
   *  @param type - The activity type: add, claim, special offer, image, video, description, testimonial
   *  @param country - The country to filter by
   *  @param latitude_1 - The latitude_1 to filter by
   *  @param longitude_1 - The longitude_1 to filter by
   *  @param latitude_2 - The latitude_2 to filter by
   *  @param longitude_2 - The longitude_2 to filter by
   *  @param number_results - The number_results to filter by
   *  @param unique_action - Return only the most recent instance of this action?
   *  @return - the data from the api
  */
  public String getActivity_stream( String type, String country, String latitude_1, String longitude_1, String latitude_2, String longitude_2, String number_results, String unique_action) {
    Hashtable p = new Hashtable();
    p.Add("type",type);
    p.Add("country",country);
    p.Add("latitude_1",latitude_1);
    p.Add("longitude_1",longitude_1);
    p.Add("latitude_2",latitude_2);
    p.Add("longitude_2",longitude_2);
    p.Add("number_results",number_results);
    p.Add("unique_action",unique_action);
    return doCurl("GET","/activity_stream",p);
  }


  /**
   * When we get some activity make a record of it
   *
   *  @param entity_id - The entity to pull
   *  @param entity_name - The entity name this entry refers to
   *  @param type - The activity type.
   *  @param country - The country for the activity
   *  @param longitude - The longitude for teh activity
   *  @param latitude - The latitude for teh activity
   *  @return - the data from the api
  */
  public String postActivity_stream( String entity_id, String entity_name, String type, String country, String longitude, String latitude) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("entity_name",entity_name);
    p.Add("type",type);
    p.Add("country",country);
    p.Add("longitude",longitude);
    p.Add("latitude",latitude);
    return doCurl("POST","/activity_stream",p);
  }


  /**
   * Get all advertisers that have been updated from a give date for a given reseller
   *
   *  @param from_date
   *  @param country
   *  @return - the data from the api
  */
  public String getAdvertiserUpdated( String from_date, String country) {
    Hashtable p = new Hashtable();
    p.Add("from_date",from_date);
    p.Add("country",country);
    return doCurl("GET","/advertiser/updated",p);
  }


  /**
   * Get all advertisers that have been updated from a give date for a given publisher
   *
   *  @param publisher_id
   *  @param from_date
   *  @param country
   *  @return - the data from the api
  */
  public String getAdvertiserUpdatedBy_publisher( String publisher_id, String from_date, String country) {
    Hashtable p = new Hashtable();
    p.Add("publisher_id",publisher_id);
    p.Add("from_date",from_date);
    p.Add("country",country);
    return doCurl("GET","/advertiser/updated/by_publisher",p);
  }


  /**
   * Update/Add a agency
   *
   *  @param agency_id
   *  @param country
   *  @param name
   *  @param description
   *  @param active
   *  @return - the data from the api
  */
  public String postAgency( String agency_id, String country, String name, String description, String active) {
    Hashtable p = new Hashtable();
    p.Add("agency_id",agency_id);
    p.Add("country",country);
    p.Add("name",name);
    p.Add("description",description);
    p.Add("active",active);
    return doCurl("POST","/agency",p);
  }


  /**
   * Returns an agency that matches a given agency id
   *
   *  @param agency_id
   *  @return - the data from the api
  */
  public String getAgency( String agency_id) {
    Hashtable p = new Hashtable();
    p.Add("agency_id",agency_id);
    return doCurl("GET","/agency",p);
  }


  /**
   * Deletes an agency that matches a given agency id
   *
   *  @param agency_id
   *  @return - the data from the api
  */
  public String deleteAgency( String agency_id) {
    Hashtable p = new Hashtable();
    p.Add("agency_id",agency_id);
    return doCurl("DELETE","/agency",p);
  }


  /**
   * The search matches a category name on a given string and language.
   *
   *  @param str - A string to search against, E.g. Plumbers e.g. but
   *  @param language - An ISO compatible language code, E.g. en e.g. en
   *  @return - the data from the api
  */
  public String getAutocompleteCategory( String str, String language) {
    Hashtable p = new Hashtable();
    p.Add("str",str);
    p.Add("language",language);
    return doCurl("GET","/autocomplete/category",p);
  }


  /**
   * The search matches a category name or synonym on a given string and language.
   *
   *  @param str - A string to search against, E.g. Plumbers e.g. but
   *  @param language - An ISO compatible language code, E.g. en e.g. en
   *  @return - the data from the api
  */
  public String getAutocompleteKeyword( String str, String language) {
    Hashtable p = new Hashtable();
    p.Add("str",str);
    p.Add("language",language);
    return doCurl("GET","/autocomplete/keyword",p);
  }


  /**
   * The search matches a location name or synonym on a given string and language.
   *
   *  @param str - A string to search against, E.g. Dub e.g. dub
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en e.g. en
   *  @return - the data from the api
  */
  public String getAutocompleteLocation( String str, String country, String language) {
    Hashtable p = new Hashtable();
    p.Add("str",str);
    p.Add("country",country);
    p.Add("language",language);
    return doCurl("GET","/autocomplete/location",p);
  }


  /**
   * The search matches a location name or synonym on a given string and language.
   *
   *  @param str - A string to search against, E.g. Middle e.g. dub
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param resolution
   *  @return - the data from the api
  */
  public String getAutocompleteLocationBy_resolution( String str, String country, String resolution) {
    Hashtable p = new Hashtable();
    p.Add("str",str);
    p.Add("country",country);
    p.Add("resolution",resolution);
    return doCurl("GET","/autocomplete/location/by_resolution",p);
  }


  /**
   * Create a new business entity with all it's objects
   *
   *  @param name
   *  @param building_number
   *  @param branch_name
   *  @param address1
   *  @param address2
   *  @param address3
   *  @param district
   *  @param town
   *  @param county
   *  @param province
   *  @param postcode
   *  @param country
   *  @param latitude
   *  @param longitude
   *  @param timezone
   *  @param telephone_number
   *  @param additional_telephone_number
   *  @param email
   *  @param website
   *  @param category_id
   *  @param category_type
   *  @param do_not_display
   *  @param referrer_url
   *  @param referrer_name
   *  @return - the data from the api
  */
  public String putBusiness( String name, String building_number, String branch_name, String address1, String address2, String address3, String district, String town, String county, String province, String postcode, String country, String latitude, String longitude, String timezone, String telephone_number, String additional_telephone_number, String email, String website, String category_id, String category_type, String do_not_display, String referrer_url, String referrer_name) {
    Hashtable p = new Hashtable();
    p.Add("name",name);
    p.Add("building_number",building_number);
    p.Add("branch_name",branch_name);
    p.Add("address1",address1);
    p.Add("address2",address2);
    p.Add("address3",address3);
    p.Add("district",district);
    p.Add("town",town);
    p.Add("county",county);
    p.Add("province",province);
    p.Add("postcode",postcode);
    p.Add("country",country);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("timezone",timezone);
    p.Add("telephone_number",telephone_number);
    p.Add("additional_telephone_number",additional_telephone_number);
    p.Add("email",email);
    p.Add("website",website);
    p.Add("category_id",category_id);
    p.Add("category_type",category_type);
    p.Add("do_not_display",do_not_display);
    p.Add("referrer_url",referrer_url);
    p.Add("referrer_name",referrer_name);
    return doCurl("PUT","/business",p);
  }


  /**
   * Returns business tool that matches a given tool id
   *
   *  @param tool_id
   *  @return - the data from the api
  */
  public String getBusiness_tool( String tool_id) {
    Hashtable p = new Hashtable();
    p.Add("tool_id",tool_id);
    return doCurl("GET","/business_tool",p);
  }


  /**
   * Delete a business tool with a specified tool_id
   *
   *  @param tool_id
   *  @return - the data from the api
  */
  public String deleteBusiness_tool( String tool_id) {
    Hashtable p = new Hashtable();
    p.Add("tool_id",tool_id);
    return doCurl("DELETE","/business_tool",p);
  }


  /**
   * Update/Add a Business Tool
   *
   *  @param tool_id
   *  @param country
   *  @param headline
   *  @param description
   *  @param link_url
   *  @param active
   *  @return - the data from the api
  */
  public String postBusiness_tool( String tool_id, String country, String headline, String description, String link_url, String active) {
    Hashtable p = new Hashtable();
    p.Add("tool_id",tool_id);
    p.Add("country",country);
    p.Add("headline",headline);
    p.Add("description",description);
    p.Add("link_url",link_url);
    p.Add("active",active);
    return doCurl("POST","/business_tool",p);
  }


  /**
   * Returns active business tools for a specific masheryid in a given country
   *
   *  @param country
   *  @return - the data from the api
  */
  public String getBusiness_toolBy_masheryid( String country) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    return doCurl("GET","/business_tool/by_masheryid",p);
  }


  /**
   * Assigns a Business Tool image
   *
   *  @param tool_id
   *  @param filedata
   *  @return - the data from the api
  */
  public String postBusiness_toolImage( String tool_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("tool_id",tool_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/business_tool/image",p);
  }


  /**
   * With a known category id, an category object can be added.
   *
   *  @param category_id
   *  @param language
   *  @param name
   *  @return - the data from the api
  */
  public String putCategory( String category_id, String language, String name) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("language",language);
    p.Add("name",name);
    return doCurl("PUT","/category",p);
  }


  /**
   * Returns the supplied wolf category object by fetching the supplied category_id from our categories object.
   *
   *  @param category_id
   *  @return - the data from the api
  */
  public String getCategory( String category_id) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    return doCurl("GET","/category",p);
  }


  /**
   * Returns all Central Index categories and associated data
   *
   *  @return - the data from the api
  */
  public String getCategoryAll() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/category/all",p);
  }


  /**
   * With a known category id, a mapping object can be deleted.
   *
   *  @param category_id
   *  @param category_type
   *  @param mapped_id
   *  @return - the data from the api
  */
  public String deleteCategoryMappings( String category_id, String category_type, String mapped_id) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("category_type",category_type);
    p.Add("mapped_id",mapped_id);
    return doCurl("DELETE","/category/mappings",p);
  }


  /**
   * With a known category id, a mapping object can be added.
   *
   *  @param category_id
   *  @param type
   *  @param id
   *  @param name
   *  @return - the data from the api
  */
  public String postCategoryMappings( String category_id, String type, String id, String name) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("type",type);
    p.Add("id",id);
    p.Add("name",name);
    return doCurl("POST","/category/mappings",p);
  }


  /**
   * Allows a category object to merged with another
   *
   *  @param from
   *  @param to
   *  @return - the data from the api
  */
  public String postCategoryMerge( String from, String to) {
    Hashtable p = new Hashtable();
    p.Add("from",from);
    p.Add("to",to);
    return doCurl("POST","/category/merge",p);
  }


  /**
   * With a known category id, a synonyms object can be removed.
   *
   *  @param category_id
   *  @param synonym
   *  @param language
   *  @return - the data from the api
  */
  public String deleteCategorySynonym( String category_id, String synonym, String language) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("synonym",synonym);
    p.Add("language",language);
    return doCurl("DELETE","/category/synonym",p);
  }


  /**
   * With a known category id, an synonym object can be added.
   *
   *  @param category_id
   *  @param synonym
   *  @param language
   *  @return - the data from the api
  */
  public String postCategorySynonym( String category_id, String synonym, String language) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("synonym",synonym);
    p.Add("language",language);
    return doCurl("POST","/category/synonym",p);
  }


  /**
   * Get the contract from the ID supplied
   *
   *  @param contract_id
   *  @return - the data from the api
  */
  public String getContract( String contract_id) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    return doCurl("GET","/contract",p);
  }


  /**
   * Get the active contracts from the ID supplied
   *
   *  @param user_id
   *  @return - the data from the api
  */
  public String getContractBy_user_id( String user_id) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    return doCurl("GET","/contract/by_user_id",p);
  }


  /**
   * Cancels an existing contract for a given id
   *
   *  @param contract_id
   *  @return - the data from the api
  */
  public String postContractCancel( String contract_id) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    return doCurl("POST","/contract/cancel",p);
  }


  /**
   * Creates a new contract for a given entity
   *
   *  @param entity_id
   *  @param user_id
   *  @param payment_provider
   *  @param basket
   *  @param billing_period
   *  @param source
   *  @param channel
   *  @param campaign
   *  @return - the data from the api
  */
  public String postContractCreate( String entity_id, String user_id, String payment_provider, String basket, String billing_period, String source, String channel, String campaign) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("user_id",user_id);
    p.Add("payment_provider",payment_provider);
    p.Add("basket",basket);
    p.Add("billing_period",billing_period);
    p.Add("source",source);
    p.Add("channel",channel);
    p.Add("campaign",campaign);
    return doCurl("POST","/contract/create",p);
  }


  /**
   * When we failed to receive money add the dates etc to the contract
   *
   *  @param contract_id
   *  @param failure_reason
   *  @param payment_date
   *  @param amount
   *  @param currency
   *  @param response
   *  @return - the data from the api
  */
  public String postContractPaymentFailure( String contract_id, String failure_reason, String payment_date, String amount, String currency, String response) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    p.Add("failure_reason",failure_reason);
    p.Add("payment_date",payment_date);
    p.Add("amount",amount);
    p.Add("currency",currency);
    p.Add("response",response);
    return doCurl("POST","/contract/payment/failure",p);
  }


  /**
   * Adds payment details to a given contract_id
   *
   *  @param contract_id
   *  @param payment_provider_id
   *  @param payment_provider_profile
   *  @param user_name
   *  @param user_surname
   *  @param user_billing_address
   *  @param user_email_address
   *  @return - the data from the api
  */
  public String postContractPaymentSetup( String contract_id, String payment_provider_id, String payment_provider_profile, String user_name, String user_surname, String user_billing_address, String user_email_address) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    p.Add("payment_provider_id",payment_provider_id);
    p.Add("payment_provider_profile",payment_provider_profile);
    p.Add("user_name",user_name);
    p.Add("user_surname",user_surname);
    p.Add("user_billing_address",user_billing_address);
    p.Add("user_email_address",user_email_address);
    return doCurl("POST","/contract/payment/setup",p);
  }


  /**
   * When we receive money add the dates etc to the contract
   *
   *  @param contract_id
   *  @param payment_date
   *  @param amount
   *  @param currency
   *  @param response
   *  @return - the data from the api
  */
  public String postContractPaymentSuccess( String contract_id, String payment_date, String amount, String currency, String response) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    p.Add("payment_date",payment_date);
    p.Add("amount",amount);
    p.Add("currency",currency);
    p.Add("response",response);
    return doCurl("POST","/contract/payment/success",p);
  }


  /**
   * Go through all the products in a contract and provision them
   *
   *  @param contract_id
   *  @return - the data from the api
  */
  public String postContractProvision( String contract_id) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    return doCurl("POST","/contract/provision",p);
  }


  /**
   * Get the contract log from the ID supplied
   *
   *  @param contract_log_id
   *  @return - the data from the api
  */
  public String getContract_log( String contract_log_id) {
    Hashtable p = new Hashtable();
    p.Add("contract_log_id",contract_log_id);
    return doCurl("GET","/contract_log",p);
  }


  /**
   * Creates a new contract log for a given contract
   *
   *  @param contract_id
   *  @param date
   *  @param payment_provider
   *  @param response
   *  @param success
   *  @param amount
   *  @param currency
   *  @return - the data from the api
  */
  public String postContract_log( String contract_id, String date, String payment_provider, String response, String success, String amount, String currency) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    p.Add("date",date);
    p.Add("payment_provider",payment_provider);
    p.Add("response",response);
    p.Add("success",success);
    p.Add("amount",amount);
    p.Add("currency",currency);
    return doCurl("POST","/contract_log",p);
  }


  /**
   * Get the contract logs from the ID supplied
   *
   *  @param contract_id
   *  @param page
   *  @param per_page
   *  @return - the data from the api
  */
  public String getContract_logBy_contract_id( String contract_id, String page, String per_page) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    p.Add("page",page);
    p.Add("per_page",per_page);
    return doCurl("GET","/contract_log/by_contract_id",p);
  }


  /**
   * Get the contract logs from the payment_provider supplied
   *
   *  @param payment_provider
   *  @param page
   *  @param per_page
   *  @return - the data from the api
  */
  public String getContract_logBy_payment_provider( String payment_provider, String page, String per_page) {
    Hashtable p = new Hashtable();
    p.Add("payment_provider",payment_provider);
    p.Add("page",page);
    p.Add("per_page",per_page);
    return doCurl("GET","/contract_log/by_payment_provider",p);
  }


  /**
   * Get the contract log from the ID supplied
   *
   *  @param date
   *  @param success
   *  @return - the data from the api
  */
  public String getContract_logSuccess_by_date( String date, String success) {
    Hashtable p = new Hashtable();
    p.Add("date",date);
    p.Add("success",success);
    return doCurl("GET","/contract_log/success_by_date",p);
  }


  /**
   * Update/Add a country
   *
   *  @param country_id
   *  @param name
   *  @param synonyms
   *  @param continentName
   *  @param continent
   *  @param geonameId
   *  @param dbpediaURL
   *  @param freebaseURL
   *  @param population
   *  @param currencyCode
   *  @param languages
   *  @param areaInSqKm
   *  @param capital
   *  @param east
   *  @param west
   *  @param north
   *  @param south
   *  @param claimPrice
   *  @param claimMethods
   *  @param twilio_sms
   *  @param twilio_phone
   *  @param twilio_voice
   *  @param currency_symbol - the symbol of this country's currency
   *  @param currency_symbol_html - the html version of the symbol of this country's currency
   *  @param postcodeLookupActive - Whether the lookup is activated for this country
   *  @param addressFields - Whether fields are activated for this country
   *  @param addressMatching - The configurable matching algorithm
   *  @param dateFormat - The format of the date for this country
   *  @param iso_3166_alpha_3
   *  @param iso_3166_numeric
   *  @return - the data from the api
  */
  public String postCountry( String country_id, String name, String synonyms, String continentName, String continent, String geonameId, String dbpediaURL, String freebaseURL, String population, String currencyCode, String languages, String areaInSqKm, String capital, String east, String west, String north, String south, String claimPrice, String claimMethods, String twilio_sms, String twilio_phone, String twilio_voice, String currency_symbol, String currency_symbol_html, String postcodeLookupActive, String addressFields, String addressMatching, String dateFormat, String iso_3166_alpha_3, String iso_3166_numeric) {
    Hashtable p = new Hashtable();
    p.Add("country_id",country_id);
    p.Add("name",name);
    p.Add("synonyms",synonyms);
    p.Add("continentName",continentName);
    p.Add("continent",continent);
    p.Add("geonameId",geonameId);
    p.Add("dbpediaURL",dbpediaURL);
    p.Add("freebaseURL",freebaseURL);
    p.Add("population",population);
    p.Add("currencyCode",currencyCode);
    p.Add("languages",languages);
    p.Add("areaInSqKm",areaInSqKm);
    p.Add("capital",capital);
    p.Add("east",east);
    p.Add("west",west);
    p.Add("north",north);
    p.Add("south",south);
    p.Add("claimPrice",claimPrice);
    p.Add("claimMethods",claimMethods);
    p.Add("twilio_sms",twilio_sms);
    p.Add("twilio_phone",twilio_phone);
    p.Add("twilio_voice",twilio_voice);
    p.Add("currency_symbol",currency_symbol);
    p.Add("currency_symbol_html",currency_symbol_html);
    p.Add("postcodeLookupActive",postcodeLookupActive);
    p.Add("addressFields",addressFields);
    p.Add("addressMatching",addressMatching);
    p.Add("dateFormat",dateFormat);
    p.Add("iso_3166_alpha_3",iso_3166_alpha_3);
    p.Add("iso_3166_numeric",iso_3166_numeric);
    return doCurl("POST","/country",p);
  }


  /**
   * Fetching a country
   *
   *  @param country_id
   *  @return - the data from the api
  */
  public String getCountry( String country_id) {
    Hashtable p = new Hashtable();
    p.Add("country_id",country_id);
    return doCurl("GET","/country",p);
  }


  /**
   * For a given country add/update a background image to show in the add/edit path
   *
   *  @param country_id
   *  @param filedata
   *  @param backgroundImageAttr
   *  @return - the data from the api
  */
  public String postCountryBackgroundImage( String country_id, String filedata, String backgroundImageAttr) {
    Hashtable p = new Hashtable();
    p.Add("country_id",country_id);
    p.Add("filedata",filedata);
    p.Add("backgroundImageAttr",backgroundImageAttr);
    return doCurl("POST","/country/backgroundImage",p);
  }


  /**
   * For a given country add/update a social login background image to show in the add/edit path
   *
   *  @param country_id
   *  @param filedata
   *  @return - the data from the api
  */
  public String postCountrySocialLoginImage( String country_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("country_id",country_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/country/socialLoginImage",p);
  }


  /**
   * Send an email via amazon
   *
   *  @param to_email_address - The email address to send the email too
   *  @param reply_email_address - The email address to add in the reply to field
   *  @param source_account - The source account to send the email from
   *  @param subject - The subject for the email
   *  @param body - The body for the email
   *  @param html_body - If the body of the email is html
   *  @return - the data from the api
  */
  public String postEmail( String to_email_address, String reply_email_address, String source_account, String subject, String body, String html_body) {
    Hashtable p = new Hashtable();
    p.Add("to_email_address",to_email_address);
    p.Add("reply_email_address",reply_email_address);
    p.Add("source_account",source_account);
    p.Add("subject",subject);
    p.Add("body",body);
    p.Add("html_body",html_body);
    return doCurl("POST","/email",p);
  }


  /**
   * Allows a whole entity to be pulled from the datastore by its unique id
   *
   *  @param entity_id - The unique entity ID e.g. 379236608286720
   *  @return - the data from the api
  */
  public String getEntity( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/entity",p);
  }


  /**
   * This entity isn't really supported anymore. You probably want PUT /business. Only to be used for testing.
   *
   *  @param type
   *  @param scope
   *  @param country
   *  @param trust
   *  @param our_data
   *  @return - the data from the api
  */
  public String putEntity( String type, String scope, String country, String trust, String our_data) {
    Hashtable p = new Hashtable();
    p.Add("type",type);
    p.Add("scope",scope);
    p.Add("country",country);
    p.Add("trust",trust);
    p.Add("our_data",our_data);
    return doCurl("PUT","/entity",p);
  }


  /**
   * With a known entity id, an add can be updated.
   *
   *  @param entity_id
   *  @param add_referrer_url
   *  @param add_referrer_name
   *  @return - the data from the api
  */
  public String postEntityAdd( String entity_id, String add_referrer_url, String add_referrer_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("add_referrer_url",add_referrer_url);
    p.Add("add_referrer_name",add_referrer_name);
    return doCurl("POST","/entity/add",p);
  }


  /**
   * Allows an advertiser object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityAdvertiser( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/advertiser",p);
  }


  /**
   * Expires an advertiser from and entity
   *
   *  @param entity_id
   *  @param publisher_id
   *  @param reseller_ref
   *  @param reseller_agent_id
   *  @return - the data from the api
  */
  public String postEntityAdvertiserCancel( String entity_id, String publisher_id, String reseller_ref, String reseller_agent_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("publisher_id",publisher_id);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_agent_id",reseller_agent_id);
    return doCurl("POST","/entity/advertiser/cancel",p);
  }


  /**
   * With a known entity id, a advertiser is added
   *
   *  @param entity_id
   *  @param tags
   *  @param locations
   *  @param max_tags
   *  @param max_locations
   *  @param expiry_date
   *  @param is_national
   *  @param language
   *  @param reseller_ref
   *  @param reseller_agent_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String postEntityAdvertiserCreate( String entity_id, String tags, String locations, String max_tags, String max_locations, String expiry_date, String is_national, String language, String reseller_ref, String reseller_agent_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("tags",tags);
    p.Add("locations",locations);
    p.Add("max_tags",max_tags);
    p.Add("max_locations",max_locations);
    p.Add("expiry_date",expiry_date);
    p.Add("is_national",is_national);
    p.Add("language",language);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_agent_id",reseller_agent_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("POST","/entity/advertiser/create",p);
  }


  /**
   * Adds/removes locations
   *
   *  @param entity_id
   *  @param gen_id
   *  @param locations_to_add
   *  @param locations_to_remove
   *  @return - the data from the api
  */
  public String postEntityAdvertiserLocation( String entity_id, String gen_id, String locations_to_add, String locations_to_remove) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    p.Add("locations_to_add",locations_to_add);
    p.Add("locations_to_remove",locations_to_remove);
    return doCurl("POST","/entity/advertiser/location",p);
  }


  /**
   * Renews an advertiser from an entity
   *
   *  @param entity_id
   *  @param expiry_date
   *  @param publisher_id
   *  @param reseller_ref
   *  @param reseller_agent_id
   *  @return - the data from the api
  */
  public String postEntityAdvertiserRenew( String entity_id, String expiry_date, String publisher_id, String reseller_ref, String reseller_agent_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("expiry_date",expiry_date);
    p.Add("publisher_id",publisher_id);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_agent_id",reseller_agent_id);
    return doCurl("POST","/entity/advertiser/renew",p);
  }


  /**
   * Allows the removal or insertion of tags into an advertiser object
   *
   *  @param gen_id - The gen_id of this advertiser
   *  @param entity_id - The entity_id of the advertiser
   *  @param language - The tag language to alter
   *  @param tags_to_add - The tags to add
   *  @param tags_to_remove - The tags to remove
   *  @return - the data from the api
  */
  public String postEntityAdvertiserTag( String gen_id, String entity_id, String language, String tags_to_add, String tags_to_remove) {
    Hashtable p = new Hashtable();
    p.Add("gen_id",gen_id);
    p.Add("entity_id",entity_id);
    p.Add("language",language);
    p.Add("tags_to_add",tags_to_add);
    p.Add("tags_to_remove",tags_to_remove);
    return doCurl("POST","/entity/advertiser/tag",p);
  }


  /**
   * With a known entity id, an advertiser is updated
   *
   *  @param entity_id
   *  @param tags
   *  @param locations
   *  @param extra_tags
   *  @param extra_locations
   *  @param is_national
   *  @param language
   *  @param reseller_ref
   *  @param reseller_agent_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String postEntityAdvertiserUpsell( String entity_id, String tags, String locations, String extra_tags, String extra_locations, String is_national, String language, String reseller_ref, String reseller_agent_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("tags",tags);
    p.Add("locations",locations);
    p.Add("extra_tags",extra_tags);
    p.Add("extra_locations",extra_locations);
    p.Add("is_national",is_national);
    p.Add("language",language);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_agent_id",reseller_agent_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("POST","/entity/advertiser/upsell",p);
  }


  /**
   * Search for matching entities that are advertisers and return a random selection upto the limit requested
   *
   *  @param tag - The word or words the advertiser is to appear for in searches
   *  @param where - The location to get results for. E.g. Dublin
   *  @param limit - The number of advertisers that are to be returned
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @return - the data from the api
  */
  public String getEntityAdvertisers( String tag, String where, String limit, String country, String language) {
    Hashtable p = new Hashtable();
    p.Add("tag",tag);
    p.Add("where",where);
    p.Add("limit",limit);
    p.Add("country",country);
    p.Add("language",language);
    return doCurl("GET","/entity/advertisers",p);
  }


  /**
   * Allows an affiliate link object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityAffiliate_link( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/affiliate_link",p);
  }


  /**
   * With a known entity id, an affiliate link object can be added.
   *
   *  @param entity_id
   *  @param affiliate_name
   *  @param affiliate_link
   *  @param affiliate_message
   *  @param affiliate_logo
   *  @return - the data from the api
  */
  public String postEntityAffiliate_link( String entity_id, String affiliate_name, String affiliate_link, String affiliate_message, String affiliate_logo) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("affiliate_name",affiliate_name);
    p.Add("affiliate_link",affiliate_link);
    p.Add("affiliate_message",affiliate_message);
    p.Add("affiliate_logo",affiliate_logo);
    return doCurl("POST","/entity/affiliate_link",p);
  }


  /**
   * Remove a agency from an entity
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityAgency( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/agency",p);
  }


  /**
   * Update/Add a agency to an entity
   *
   *  @param entity_id
   *  @param agency_id
   *  @return - the data from the api
  */
  public String postEntityAgency( String entity_id, String agency_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("agency_id",agency_id);
    return doCurl("POST","/entity/agency",p);
  }


  /**
   * With a known entity id, an background object can be added. There can however only be one background object.
   *
   *  @param entity_id
   *  @param number_of_employees
   *  @param turnover
   *  @param net_profit
   *  @param vat_number
   *  @param duns_number
   *  @param registered_company_number
   *  @return - the data from the api
  */
  public String postEntityBackground( String entity_id, String number_of_employees, String turnover, String net_profit, String vat_number, String duns_number, String registered_company_number) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("number_of_employees",number_of_employees);
    p.Add("turnover",turnover);
    p.Add("net_profit",net_profit);
    p.Add("vat_number",vat_number);
    p.Add("duns_number",duns_number);
    p.Add("registered_company_number",registered_company_number);
    return doCurl("POST","/entity/background",p);
  }


  /**
   * Uploads a CSV file of known format and bulk inserts into DB
   *
   *  @param filedata
   *  @return - the data from the api
  */
  public String postEntityBulkCsv( String filedata) {
    Hashtable p = new Hashtable();
    p.Add("filedata",filedata);
    return doCurl("POST","/entity/bulk/csv",p);
  }


  /**
   * Shows the current status of a bulk upload
   *
   *  @param upload_id
   *  @return - the data from the api
  */
  public String getEntityBulkCsvStatus( String upload_id) {
    Hashtable p = new Hashtable();
    p.Add("upload_id",upload_id);
    return doCurl("GET","/entity/bulk/csv/status",p);
  }


  /**
   * Uploads a JSON file of known format and bulk inserts into DB
   *
   *  @param data
   *  @return - the data from the api
  */
  public String postEntityBulkJson( String data) {
    Hashtable p = new Hashtable();
    p.Add("data",data);
    return doCurl("POST","/entity/bulk/json",p);
  }


  /**
   * Shows the current status of a bulk JSON upload
   *
   *  @param upload_id
   *  @return - the data from the api
  */
  public String getEntityBulkJsonStatus( String upload_id) {
    Hashtable p = new Hashtable();
    p.Add("upload_id",upload_id);
    return doCurl("GET","/entity/bulk/json/status",p);
  }


  /**
   * Get all entities managed by a specified agency
   *
   *  @param agency_id
   *  @return - the data from the api
  */
  public String getEntityBy_agencyid( String agency_id) {
    Hashtable p = new Hashtable();
    p.Add("agency_id",agency_id);
    return doCurl("GET","/entity/by_agencyid",p);
  }


  /**
   * Get all entities within a specified group
   *
   *  @param group_id
   *  @return - the data from the api
  */
  public String getEntityBy_groupid( String group_id) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    return doCurl("GET","/entity/by_groupid",p);
  }


  /**
   * uncontributes a given entities supplier content and makes the entity inactive if the entity is un-usable
   *
   *  @param entity_id - The entity to pull
   *  @param supplier_masheryid - The suppliers masheryid to match
   *  @param supplier_id - The supplier id to match
   *  @param supplier_user_id - The user id to match
   *  @return - the data from the api
  */
  public String deleteEntityBy_supplier( String entity_id, String supplier_masheryid, String supplier_id, String supplier_user_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("supplier_masheryid",supplier_masheryid);
    p.Add("supplier_id",supplier_id);
    p.Add("supplier_user_id",supplier_user_id);
    return doCurl("DELETE","/entity/by_supplier",p);
  }


  /**
   * Fetches the documents that match the given masheryid and supplier_id
   *
   *  @param supplier_id - The Supplier ID
   *  @return - the data from the api
  */
  public String getEntityBy_supplier_id( String supplier_id) {
    Hashtable p = new Hashtable();
    p.Add("supplier_id",supplier_id);
    return doCurl("GET","/entity/by_supplier_id",p);
  }


  /**
   * Get all entiies claimed by a specific user
   *
   *  @param user_id - The unique user ID of the user with claimed entities e.g. 379236608286720
   *  @return - the data from the api
  */
  public String getEntityBy_user_id( String user_id) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    return doCurl("GET","/entity/by_user_id",p);
  }


  /**
   * Allows a category object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityCategory( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/category",p);
  }


  /**
   * With a known entity id, an category object can be added.
   *
   *  @param entity_id
   *  @param category_id
   *  @param category_type
   *  @return - the data from the api
  */
  public String postEntityCategory( String entity_id, String category_id, String category_type) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("category_id",category_id);
    p.Add("category_type",category_type);
    return doCurl("POST","/entity/category",p);
  }


  /**
   * Fetches the changelog documents that match the given entity_id
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String getEntityChangelog( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/entity/changelog",p);
  }


  /**
   * Allow an entity to be claimed by a valid user
   *
   *  @param entity_id
   *  @param claimed_user_id
   *  @param claimed_date
   *  @param claim_method
   *  @param phone_number
   *  @param referrer_url
   *  @param referrer_name
   *  @return - the data from the api
  */
  public String postEntityClaim( String entity_id, String claimed_user_id, String claimed_date, String claim_method, String phone_number, String referrer_url, String referrer_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("claimed_user_id",claimed_user_id);
    p.Add("claimed_date",claimed_date);
    p.Add("claim_method",claim_method);
    p.Add("phone_number",phone_number);
    p.Add("referrer_url",referrer_url);
    p.Add("referrer_name",referrer_name);
    return doCurl("POST","/entity/claim",p);
  }


  /**
   * Allows a description object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityDescription( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/description",p);
  }


  /**
   * With a known entity id, a description object can be added.
   *
   *  @param entity_id
   *  @param headline
   *  @param body
   *  @return - the data from the api
  */
  public String postEntityDescription( String entity_id, String headline, String body) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("headline",headline);
    p.Add("body",body);
    return doCurl("POST","/entity/description",p);
  }


  /**
   * With a known entity id, an document object can be added.
   *
   *  @param entity_id
   *  @param name
   *  @param filedata
   *  @return - the data from the api
  */
  public String postEntityDocument( String entity_id, String name, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("name",name);
    p.Add("filedata",filedata);
    return doCurl("POST","/entity/document",p);
  }


  /**
   * Allows a phone object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityDocument( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/document",p);
  }


  /**
   * Allows a email object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityEmail( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/email",p);
  }


  /**
   * With a known entity id, an email address object can be added.
   *
   *  @param entity_id
   *  @param email_address
   *  @param email_description
   *  @return - the data from the api
  */
  public String postEntityEmail( String entity_id, String email_address, String email_description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("email_address",email_address);
    p.Add("email_description",email_description);
    return doCurl("POST","/entity/email",p);
  }


  /**
   * Allows an employee object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityEmployee( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/employee",p);
  }


  /**
   * With a known entity id, an employee object can be added.
   *
   *  @param entity_id
   *  @param title
   *  @param forename
   *  @param surname
   *  @param job_title
   *  @param description
   *  @param email
   *  @param phone_number
   *  @return - the data from the api
  */
  public String postEntityEmployee( String entity_id, String title, String forename, String surname, String job_title, String description, String email, String phone_number) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("title",title);
    p.Add("forename",forename);
    p.Add("surname",surname);
    p.Add("job_title",job_title);
    p.Add("description",description);
    p.Add("email",email);
    p.Add("phone_number",phone_number);
    return doCurl("POST","/entity/employee",p);
  }


  /**
   * Allows a fax object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityFax( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/fax",p);
  }


  /**
   * With a known entity id, an fax object can be added.
   *
   *  @param entity_id
   *  @param number
   *  @param description
   *  @return - the data from the api
  */
  public String postEntityFax( String entity_id, String number, String description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("number",number);
    p.Add("description",description);
    return doCurl("POST","/entity/fax",p);
  }


  /**
   * With a known entity id, a geopoint can be updated.
   *
   *  @param entity_id
   *  @param longitude
   *  @param latitude
   *  @param accuracy
   *  @return - the data from the api
  */
  public String postEntityGeopoint( String entity_id, String longitude, String latitude, String accuracy) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("longitude",longitude);
    p.Add("latitude",latitude);
    p.Add("accuracy",accuracy);
    return doCurl("POST","/entity/geopoint",p);
  }


  /**
   * Allows a group object to be removed from an entities group members
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityGroup( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/group",p);
  }


  /**
   * With a known entity id, a group  can be added to group members.
   *
   *  @param entity_id
   *  @param group_id
   *  @return - the data from the api
  */
  public String postEntityGroup( String entity_id, String group_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("group_id",group_id);
    return doCurl("POST","/entity/group",p);
  }


  /**
   * With a known entity id, a image object can be added.
   *
   *  @param entity_id
   *  @param filedata
   *  @param image_name
   *  @return - the data from the api
  */
  public String postEntityImage( String entity_id, String filedata, String image_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("filedata",filedata);
    p.Add("image_name",image_name);
    return doCurl("POST","/entity/image",p);
  }


  /**
   * Allows a image object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityImage( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/image",p);
  }


  /**
   * With a known entity id, an invoice_address object can be updated.
   *
   *  @param entity_id
   *  @param building_number
   *  @param address1
   *  @param address2
   *  @param address3
   *  @param district
   *  @param town
   *  @param county
   *  @param province
   *  @param postcode
   *  @param address_type
   *  @return - the data from the api
  */
  public String postEntityInvoice_address( String entity_id, String building_number, String address1, String address2, String address3, String district, String town, String county, String province, String postcode, String address_type) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("building_number",building_number);
    p.Add("address1",address1);
    p.Add("address2",address2);
    p.Add("address3",address3);
    p.Add("district",district);
    p.Add("town",town);
    p.Add("county",county);
    p.Add("province",province);
    p.Add("postcode",postcode);
    p.Add("address_type",address_type);
    return doCurl("POST","/entity/invoice_address",p);
  }


  /**
   * With a known entity id and a known invoice_address ID, we can delete a specific invoice_address object from an enitity.
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String deleteEntityInvoice_address( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/entity/invoice_address",p);
  }


  /**
   * With a known entity id, a list description object can be added.
   *
   *  @param entity_id
   *  @param headline
   *  @param body
   *  @return - the data from the api
  */
  public String postEntityList( String entity_id, String headline, String body) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("headline",headline);
    p.Add("body",body);
    return doCurl("POST","/entity/list",p);
  }


  /**
   * Allows a list description object to be reduced in confidence
   *
   *  @param gen_id
   *  @param entity_id
   *  @return - the data from the api
  */
  public String deleteEntityList( String gen_id, String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("gen_id",gen_id);
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/entity/list",p);
  }


  /**
   * Allows a phone object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityLogo( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/logo",p);
  }


  /**
   * With a known entity id, a logo object can be added.
   *
   *  @param entity_id
   *  @param filedata
   *  @param logo_name
   *  @return - the data from the api
  */
  public String postEntityLogo( String entity_id, String filedata, String logo_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("filedata",filedata);
    p.Add("logo_name",logo_name);
    return doCurl("POST","/entity/logo",p);
  }


  /**
   * Merge two entities into one
   *
   *  @param from
   *  @param to
   *  @param override_trust - Do you want to override the trust of the 'from' entity
   *  @param uncontribute_masheryid - Do we want to uncontribute any data for a masheryid?
   *  @param uncontribute_userid - Do we want to uncontribute any data for a user_id?
   *  @param uncontribute_supplierid - Do we want to uncontribute any data for a supplier_id?
   *  @return - the data from the api
  */
  public String postEntityMerge( String from, String to, String override_trust, String uncontribute_masheryid, String uncontribute_userid, String uncontribute_supplierid) {
    Hashtable p = new Hashtable();
    p.Add("from",from);
    p.Add("to",to);
    p.Add("override_trust",override_trust);
    p.Add("uncontribute_masheryid",uncontribute_masheryid);
    p.Add("uncontribute_userid",uncontribute_userid);
    p.Add("uncontribute_supplierid",uncontribute_supplierid);
    return doCurl("POST","/entity/merge",p);
  }


  /**
   * Update entities that use an old category ID to a new one
   *
   *  @param from
   *  @param to
   *  @param limit
   *  @return - the data from the api
  */
  public String postEntityMigrate_category( String from, String to, String limit) {
    Hashtable p = new Hashtable();
    p.Add("from",from);
    p.Add("to",to);
    p.Add("limit",limit);
    return doCurl("POST","/entity/migrate_category",p);
  }


  /**
   * With a known entity id, a name can be updated.
   *
   *  @param entity_id
   *  @param name
   *  @param formal_name
   *  @param branch_name
   *  @return - the data from the api
  */
  public String postEntityName( String entity_id, String name, String formal_name, String branch_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("name",name);
    p.Add("formal_name",formal_name);
    p.Add("branch_name",branch_name);
    return doCurl("POST","/entity/name",p);
  }


  /**
   * With a known entity id, a opening times object can be removed.
   *
   *  @param entity_id - The id of the entity to edit
   *  @return - the data from the api
  */
  public String deleteEntityOpening_times( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/entity/opening_times",p);
  }


  /**
   * With a known entity id, a opening times object can be added. Each day can be either 'closed' to indicate that the entity is closed that day, '24hour' to indicate that the entity is open all day or single/split time ranges can be supplied in 4-digit 24-hour format, such as '09001730' or '09001200,13001700' to indicate hours of opening.
   *
   *  @param entity_id - The id of the entity to edit
   *  @param monday - e.g. 'closed', '24hour' , '09001730' , '09001200,13001700'
   *  @param tuesday - e.g. 'closed', '24hour' , '09001730' , '09001200,13001700'
   *  @param wednesday - e.g. 'closed', '24hour' , '09001730' , '09001200,13001700'
   *  @param thursday - e.g. 'closed', '24hour' , '09001730' , '09001200,13001700'
   *  @param friday - e.g. 'closed', '24hour' , '09001730' , '09001200,13001700'
   *  @param saturday - e.g. 'closed', '24hour' , '09001730' , '09001200,13001700'
   *  @param sunday - e.g. 'closed', '24hour' , '09001730' , '09001200,13001700'
   *  @param closed - a comma-separated list of dates that the entity is closed e.g. '2013-04-29,2013-05-02'
   *  @param closed_public_holidays - whether the entity is closed on public holidays
   *  @return - the data from the api
  */
  public String postEntityOpening_times( String entity_id, String monday, String tuesday, String wednesday, String thursday, String friday, String saturday, String sunday, String closed, String closed_public_holidays) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("monday",monday);
    p.Add("tuesday",tuesday);
    p.Add("wednesday",wednesday);
    p.Add("thursday",thursday);
    p.Add("friday",friday);
    p.Add("saturday",saturday);
    p.Add("sunday",sunday);
    p.Add("closed",closed);
    p.Add("closed_public_holidays",closed_public_holidays);
    return doCurl("POST","/entity/opening_times",p);
  }


  /**
   * Allows a new phone object to be added to a specified entity. A new object id will be calculated and returned to you if successful.
   *
   *  @param entity_id
   *  @param number
   *  @param trackable
   *  @return - the data from the api
  */
  public String postEntityPhone( String entity_id, String number, String trackable) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("number",number);
    p.Add("trackable",trackable);
    return doCurl("POST","/entity/phone",p);
  }


  /**
   * Allows a phone object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityPhone( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/phone",p);
  }


  /**
   * Create/Update a postal address
   *
   *  @param entity_id
   *  @param building_number
   *  @param address1
   *  @param address2
   *  @param address3
   *  @param district
   *  @param town
   *  @param county
   *  @param province
   *  @param postcode
   *  @param address_type
   *  @param do_not_display
   *  @return - the data from the api
  */
  public String postEntityPostal_address( String entity_id, String building_number, String address1, String address2, String address3, String district, String town, String county, String province, String postcode, String address_type, String do_not_display) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("building_number",building_number);
    p.Add("address1",address1);
    p.Add("address2",address2);
    p.Add("address3",address3);
    p.Add("district",district);
    p.Add("town",town);
    p.Add("county",county);
    p.Add("province",province);
    p.Add("postcode",postcode);
    p.Add("address_type",address_type);
    p.Add("do_not_display",do_not_display);
    return doCurl("POST","/entity/postal_address",p);
  }


  /**
   * Fetches the documents that match the given masheryid and supplier_id
   *
   *  @param supplier_id - The Supplier ID
   *  @return - the data from the api
  */
  public String getEntityProvisionalBy_supplier_id( String supplier_id) {
    Hashtable p = new Hashtable();
    p.Add("supplier_id",supplier_id);
    return doCurl("GET","/entity/provisional/by_supplier_id",p);
  }


  /**
   * Allows a list of available revisions to be returned by its entity id
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String getEntityRevisions( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/entity/revisions",p);
  }


  /**
   * Allows a specific revision of an entity to be returned by entity id and a revision number
   *
   *  @param entity_id
   *  @param revision_id
   *  @return - the data from the api
  */
  public String getEntityRevisionsByRevisionID( String entity_id, String revision_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("revision_id",revision_id);
    return doCurl("GET","/entity/revisions/byRevisionID",p);
  }


  /**
   * Search for matching entities
   *
   *  @param latitude_1
   *  @param longitude_1
   *  @param latitude_2
   *  @param longitude_2
   *  @param per_page
   *  @param page
   *  @param country
   *  @param language
   *  @return - the data from the api
  */
  public String getEntitySearchByboundingbox( String latitude_1, String longitude_1, String latitude_2, String longitude_2, String per_page, String page, String country, String language) {
    Hashtable p = new Hashtable();
    p.Add("latitude_1",latitude_1);
    p.Add("longitude_1",longitude_1);
    p.Add("latitude_2",latitude_2);
    p.Add("longitude_2",longitude_2);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    return doCurl("GET","/entity/search/byboundingbox",p);
  }


  /**
   * Search for matching entities
   *
   *  @param where - Location to search for results. E.g. Dublin e.g. Dublin
   *  @param per_page - How many results per page
   *  @param page - What page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @param latitude - The decimal latitude of the search context (optional)
   *  @param longitude - The decimal longitude of the search context (optional)
   *  @return - the data from the api
  */
  public String getEntitySearchBylocation( String where, String per_page, String page, String country, String language, String latitude, String longitude) {
    Hashtable p = new Hashtable();
    p.Add("where",where);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    return doCurl("GET","/entity/search/bylocation",p);
  }


  /**
   * Search for matching entities
   *
   *  @param what - What to get results for. E.g. Plumber e.g. plumber
   *  @param per_page - Number of results returned per page
   *  @param page - The page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @return - the data from the api
  */
  public String getEntitySearchWhat( String what, String per_page, String page, String country, String language) {
    Hashtable p = new Hashtable();
    p.Add("what",what);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    return doCurl("GET","/entity/search/what",p);
  }


  /**
   * Search for matching entities
   *
   *  @param what - What to get results for. E.g. Plumber e.g. plumber
   *  @param latitude_1 - Latitude of first point in bounding box e.g. 53.396842
   *  @param longitude_1 - Longitude of first point in bounding box e.g. -6.37619
   *  @param latitude_2 - Latitude of second point in bounding box e.g. 53.290463
   *  @param longitude_2 - Longitude of second point in bounding box e.g. -6.207275
   *  @param per_page
   *  @param page
   *  @param country - A valid ISO 3166 country code e.g. ie
   *  @param language
   *  @return - the data from the api
  */
  public String getEntitySearchWhatByboundingbox( String what, String latitude_1, String longitude_1, String latitude_2, String longitude_2, String per_page, String page, String country, String language) {
    Hashtable p = new Hashtable();
    p.Add("what",what);
    p.Add("latitude_1",latitude_1);
    p.Add("longitude_1",longitude_1);
    p.Add("latitude_2",latitude_2);
    p.Add("longitude_2",longitude_2);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    return doCurl("GET","/entity/search/what/byboundingbox",p);
  }


  /**
   * Search for matching entities
   *
   *  @param what - What to get results for. E.g. Plumber e.g. plumber
   *  @param where - The location to get results for. E.g. Dublin e.g. Dublin
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @param latitude - The decimal latitude of the search context (optional)
   *  @param longitude - The decimal longitude of the search context (optional)
   *  @return - the data from the api
  */
  public String getEntitySearchWhatBylocation( String what, String where, String per_page, String page, String country, String language, String latitude, String longitude) {
    Hashtable p = new Hashtable();
    p.Add("what",what);
    p.Add("where",where);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    return doCurl("GET","/entity/search/what/bylocation",p);
  }


  /**
   * Search for matching entities, ordered by nearness
   *
   *  @param what - What to get results for. E.g. Plumber e.g. plumber
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @param language - An ISO compatible language code, E.g. en
   *  @param latitude - The decimal latitude of the centre point of the search
   *  @param longitude - The decimal longitude of the centre point of the search
   *  @return - the data from the api
  */
  public String getEntitySearchWhatBynearest( String what, String per_page, String page, String language, String latitude, String longitude) {
    Hashtable p = new Hashtable();
    p.Add("what",what);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("language",language);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    return doCurl("GET","/entity/search/what/bynearest",p);
  }


  /**
   * Search for matching entities
   *
   *  @param who - Company name e.g. Starbucks
   *  @param per_page - How many results per page
   *  @param page - What page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @return - the data from the api
  */
  public String getEntitySearchWho( String who, String per_page, String page, String country, String language) {
    Hashtable p = new Hashtable();
    p.Add("who",who);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    return doCurl("GET","/entity/search/who",p);
  }


  /**
   * Search for matching entities
   *
   *  @param who
   *  @param latitude_1
   *  @param longitude_1
   *  @param latitude_2
   *  @param longitude_2
   *  @param per_page
   *  @param page
   *  @param country
   *  @param language - An ISO compatible language code, E.g. en
   *  @return - the data from the api
  */
  public String getEntitySearchWhoByboundingbox( String who, String latitude_1, String longitude_1, String latitude_2, String longitude_2, String per_page, String page, String country, String language) {
    Hashtable p = new Hashtable();
    p.Add("who",who);
    p.Add("latitude_1",latitude_1);
    p.Add("longitude_1",longitude_1);
    p.Add("latitude_2",latitude_2);
    p.Add("longitude_2",longitude_2);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    return doCurl("GET","/entity/search/who/byboundingbox",p);
  }


  /**
   * Search for matching entities
   *
   *  @param who - Company Name e.g. Starbucks
   *  @param where - The location to get results for. E.g. Dublin e.g. Dublin
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param latitude - The decimal latitude of the search context (optional)
   *  @param longitude - The decimal longitude of the search context (optional)
   *  @param language - An ISO compatible language code, E.g. en
   *  @return - the data from the api
  */
  public String getEntitySearchWhoBylocation( String who, String where, String per_page, String page, String country, String latitude, String longitude, String language) {
    Hashtable p = new Hashtable();
    p.Add("who",who);
    p.Add("where",where);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("language",language);
    return doCurl("GET","/entity/search/who/bylocation",p);
  }


  /**
   * Send an email to an email address specified in an entity
   *
   *  @param entity_id - The entity id of the entity you wish to contact
   *  @param gen_id - The gen_id of the email address you wish to send the message to
   *  @param from_email - The email of the person sending the message 
   *  @param subject - The subject for the email
   *  @param content - The content of the email
   *  @return - the data from the api
  */
  public String postEntitySend_email( String entity_id, String gen_id, String from_email, String subject, String content) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    p.Add("from_email",from_email);
    p.Add("subject",subject);
    p.Add("content",content);
    return doCurl("POST","/entity/send_email",p);
  }


  /**
   * Allows a social media object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntitySocialmedia( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/socialmedia",p);
  }


  /**
   * With a known entity id, a social media object can be added.
   *
   *  @param entity_id
   *  @param type
   *  @param website_url
   *  @return - the data from the api
  */
  public String postEntitySocialmedia( String entity_id, String type, String website_url) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("type",type);
    p.Add("website_url",website_url);
    return doCurl("POST","/entity/socialmedia",p);
  }


  /**
   * With a known entity id, a website object can be added.
   *
   *  @param entity_id
   *  @param title
   *  @param description
   *  @param terms
   *  @param start_date
   *  @param expiry_date
   *  @param url
   *  @param image_url
   *  @return - the data from the api
  */
  public String postEntitySpecial_offer( String entity_id, String title, String description, String terms, String start_date, String expiry_date, String url, String image_url) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("title",title);
    p.Add("description",description);
    p.Add("terms",terms);
    p.Add("start_date",start_date);
    p.Add("expiry_date",expiry_date);
    p.Add("url",url);
    p.Add("image_url",image_url);
    return doCurl("POST","/entity/special_offer",p);
  }


  /**
   * Allows a special offer object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntitySpecial_offer( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/special_offer",p);
  }


  /**
   * With a known entity id, a status object can be updated.
   *
   *  @param entity_id
   *  @param status
   *  @param inactive_reason
   *  @param inactive_description
   *  @return - the data from the api
  */
  public String postEntityStatus( String entity_id, String status, String inactive_reason, String inactive_description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("status",status);
    p.Add("inactive_reason",inactive_reason);
    p.Add("inactive_description",inactive_description);
    return doCurl("POST","/entity/status",p);
  }


  /**
   * With a known entity id, an tag object can be added.
   *
   *  @param entity_id
   *  @param tag
   *  @param language
   *  @return - the data from the api
  */
  public String postEntityTag( String entity_id, String tag, String language) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("tag",tag);
    p.Add("language",language);
    return doCurl("POST","/entity/tag",p);
  }


  /**
   * Allows a tag object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityTag( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/tag",p);
  }


  /**
   * With a known entity id, a testimonial object can be added.
   *
   *  @param entity_id
   *  @param title
   *  @param text
   *  @param date
   *  @param testifier_name
   *  @return - the data from the api
  */
  public String postEntityTestimonial( String entity_id, String title, String text, String date, String testifier_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("title",title);
    p.Add("text",text);
    p.Add("date",date);
    p.Add("testifier_name",testifier_name);
    return doCurl("POST","/entity/testimonial",p);
  }


  /**
   * Allows a testimonial object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityTestimonial( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/testimonial",p);
  }


  /**
   * Get the updates a uncontribute would perform
   *
   *  @param entity_id - The entity to pull
   *  @param object_name - The entity object to update
   *  @param supplier_id - The supplier_id to remove
   *  @param user_id - The user_id to remove
   *  @return - the data from the api
  */
  public String getEntityUncontribute( String entity_id, String object_name, String supplier_id, String user_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("object_name",object_name);
    p.Add("supplier_id",supplier_id);
    p.Add("user_id",user_id);
    return doCurl("GET","/entity/uncontribute",p);
  }


  /**
   * Separates an entity into two distinct entities 
   *
   *  @param entity_id
   *  @param supplier_masheryid
   *  @param supplier_id
   *  @return - the data from the api
  */
  public String postEntityUnmerge( String entity_id, String supplier_masheryid, String supplier_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("supplier_masheryid",supplier_masheryid);
    p.Add("supplier_id",supplier_id);
    return doCurl("POST","/entity/unmerge",p);
  }


  /**
   * Allows a video object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityVideo( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/video",p);
  }


  /**
   * With a known entity id, a YouTube video object can be added.
   *
   *  @param entity_id
   *  @param embed_code
   *  @return - the data from the api
  */
  public String postEntityVideoYoutube( String entity_id, String embed_code) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("embed_code",embed_code);
    return doCurl("POST","/entity/video/youtube",p);
  }


  /**
   * With a known entity id, a website object can be added.
   *
   *  @param entity_id
   *  @param website_url
   *  @param display_url
   *  @param website_description
   *  @return - the data from the api
  */
  public String postEntityWebsite( String entity_id, String website_url, String display_url, String website_description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("website_url",website_url);
    p.Add("display_url",display_url);
    p.Add("website_description",website_description);
    return doCurl("POST","/entity/website",p);
  }


  /**
   * Allows a website object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteEntityWebsite( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/website",p);
  }


  /**
   * Add an entityserve document
   *
   *  @param entity_id - The id of the entity to create the entityserve event for
   *  @param country - the ISO code of the country
   *  @param event_type - The event type being recorded
   *  @return - the data from the api
  */
  public String putEntityserve( String entity_id, String country, String event_type) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("country",country);
    p.Add("event_type",event_type);
    return doCurl("PUT","/entityserve",p);
  }


  /**
   * Remove a flatpack using a supplied flatpack_id
   *
   *  @param flatpack_id - the id of the flatpack to delete
   *  @return - the data from the api
  */
  public String deleteFlatpack( String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    return doCurl("DELETE","/flatpack",p);
  }


  /**
   * Get a flatpack
   *
   *  @param flatpack_id - the unique id to search for
   *  @return - the data from the api
  */
  public String getFlatpack( String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/flatpack",p);
  }


  /**
   * Update/Add a flatpack
   *
   *  @param flatpack_id - this record's unique, auto-generated id - if supplied, then this is an edit, otherwise it's an add
   *  @param domainName - the domain name to serve this flatpack site on (no leading http:// or anything please)
   *  @param stub - the stub that is appended to the flatpack's url e.g. http://dev.localhost/stub
   *  @param flatpackName - the name of the Flat pack instance
   *  @param less - the LESS configuration to use to overrides the Bootstrap CSS
   *  @param language - the language in which to render the flatpack site
   *  @param country - the country to use for searches etc
   *  @param mapsType - the type of maps to use
   *  @param mapKey - the nokia map key to use to render maps
   *  @param searchFormShowOn - list of pages to show the search form
   *  @param searchFormShowKeywordsBox - whether to display the keywords box on the search form
   *  @param searchFormShowLocationBox - whether to display the location box on search forms - not required
   *  @param searchFormKeywordsAutoComplete - whether to do auto-completion on the keywords box on the search form
   *  @param searchFormLocationsAutoComplete - whether to do auto-completion on the locations box on the search form
   *  @param searchFormDefaultLocation - the string to use as the default location for searches if no location is supplied
   *  @param searchFormPlaceholderKeywords - the string to show in the keyword box as placeholder text e.g. e.g. cafe
   *  @param searchFormPlaceholderLocation - the string to show in the location box as placeholder text e.g. e.g. Dublin
   *  @param searchFormKeywordsLabel - the string to show next to the keywords control e.g. I'm looking for
   *  @param searchFormLocationLabel - the string to show next to the location control e.g. Located in
   *  @param cannedLinksHeader - the string to show above canned searches
   *  @param homepageTitle - the page title of site's home page
   *  @param homepageDescription - the meta description of the home page
   *  @param homepageIntroTitle - the introductory title for the homepage
   *  @param homepageIntroText - the introductory text for the homepage
   *  @param head - payload to put in the head of the flatpack
   *  @param adblock - payload to put in the adblock of the flatpack
   *  @param bodyTop - the payload to put in the top of the body of a flatpack
   *  @param bodyBottom - the payload to put in the bottom of the body of a flatpack
   *  @param header_menu - the JSON that describes a navigation at the top of the page
   *  @param header_menu_bottom - the JSON that describes a navigation below the masthead
   *  @param footer_menu - the JSON that describes a navigation at the bottom of the page
   *  @param bdpTitle - The page title of the entity business profile pages
   *  @param bdpDescription - The meta description of entity business profile pages
   *  @param bdpAds - The block of HTML/JS that renders Ads on BDPs
   *  @param serpTitle - The page title of the serps
   *  @param serpDescription - The meta description of serps
   *  @param serpNumberResults - The number of results per search page
   *  @param serpNumberAdverts - The number of adverts to show on the first search page
   *  @param serpAds - The block of HTML/JS that renders Ads on Serps
   *  @param serpTitleNoWhat - The text to display in the title for where only searches
   *  @param serpDescriptionNoWhat - The text to display in the description for where only searches
   *  @param cookiePolicyUrl - The cookie policy url of the flatpack
   *  @param cookiePolicyNotice - Whether to show the cookie policy on this flatpack
   *  @param addBusinessButtonText - The text used in the 'Add your business' button
   *  @param twitterUrl - Twitter link
   *  @param facebookUrl - Facebook link
   *  @param copyright - Copyright message
   *  @param advertUpgradeActive - whether upgrade message is displayed on this Flatpack
   *  @param advertUpgradePrice - the cost of upgrading
   *  @param advertUpgradeMaxTags - the number of tags upgrading gives you
   *  @param advertUpgradeMaxLocations - the number of locations upgrading gives you
   *  @param advertUpgradeContractLength - the length of the contract (days)
   *  @param advertUpgradeRefId - a unique reference for the upgrade
   *  @param phoneReveal - record phone number reveal
   *  @param loginLinkText - the link text for the Login link
   *  @param contextLocationId - The location ID to use as the context for searches on this flatpack
   *  @param addBusinessButtonPosition - The location ID to use as the context for searches on this flatpack
   *  @param denyIndexing - Whether to noindex a flatpack
   *  @param contextRadius - allows you to set a catchment area around the contextLocationId in miles for use when displaying the activity stream module
   *  @param activityStream - allows you to set the activity to be displayed in the activity stream
   *  @param activityStreamSize - Sets the number of items to show within the activity stream.
   *  @return - the data from the api
  */
  public String postFlatpack( String flatpack_id, String domainName, String stub, String flatpackName, String less, String language, String country, String mapsType, String mapKey, String searchFormShowOn, String searchFormShowKeywordsBox, String searchFormShowLocationBox, String searchFormKeywordsAutoComplete, String searchFormLocationsAutoComplete, String searchFormDefaultLocation, String searchFormPlaceholderKeywords, String searchFormPlaceholderLocation, String searchFormKeywordsLabel, String searchFormLocationLabel, String cannedLinksHeader, String homepageTitle, String homepageDescription, String homepageIntroTitle, String homepageIntroText, String head, String adblock, String bodyTop, String bodyBottom, String header_menu, String header_menu_bottom, String footer_menu, String bdpTitle, String bdpDescription, String bdpAds, String serpTitle, String serpDescription, String serpNumberResults, String serpNumberAdverts, String serpAds, String serpTitleNoWhat, String serpDescriptionNoWhat, String cookiePolicyUrl, String cookiePolicyNotice, String addBusinessButtonText, String twitterUrl, String facebookUrl, String copyright, String advertUpgradeActive, String advertUpgradePrice, String advertUpgradeMaxTags, String advertUpgradeMaxLocations, String advertUpgradeContractLength, String advertUpgradeRefId, String phoneReveal, String loginLinkText, String contextLocationId, String addBusinessButtonPosition, String denyIndexing, String contextRadius, String activityStream, String activityStreamSize) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("domainName",domainName);
    p.Add("stub",stub);
    p.Add("flatpackName",flatpackName);
    p.Add("less",less);
    p.Add("language",language);
    p.Add("country",country);
    p.Add("mapsType",mapsType);
    p.Add("mapKey",mapKey);
    p.Add("searchFormShowOn",searchFormShowOn);
    p.Add("searchFormShowKeywordsBox",searchFormShowKeywordsBox);
    p.Add("searchFormShowLocationBox",searchFormShowLocationBox);
    p.Add("searchFormKeywordsAutoComplete",searchFormKeywordsAutoComplete);
    p.Add("searchFormLocationsAutoComplete",searchFormLocationsAutoComplete);
    p.Add("searchFormDefaultLocation",searchFormDefaultLocation);
    p.Add("searchFormPlaceholderKeywords",searchFormPlaceholderKeywords);
    p.Add("searchFormPlaceholderLocation",searchFormPlaceholderLocation);
    p.Add("searchFormKeywordsLabel",searchFormKeywordsLabel);
    p.Add("searchFormLocationLabel",searchFormLocationLabel);
    p.Add("cannedLinksHeader",cannedLinksHeader);
    p.Add("homepageTitle",homepageTitle);
    p.Add("homepageDescription",homepageDescription);
    p.Add("homepageIntroTitle",homepageIntroTitle);
    p.Add("homepageIntroText",homepageIntroText);
    p.Add("head",head);
    p.Add("adblock",adblock);
    p.Add("bodyTop",bodyTop);
    p.Add("bodyBottom",bodyBottom);
    p.Add("header_menu",header_menu);
    p.Add("header_menu_bottom",header_menu_bottom);
    p.Add("footer_menu",footer_menu);
    p.Add("bdpTitle",bdpTitle);
    p.Add("bdpDescription",bdpDescription);
    p.Add("bdpAds",bdpAds);
    p.Add("serpTitle",serpTitle);
    p.Add("serpDescription",serpDescription);
    p.Add("serpNumberResults",serpNumberResults);
    p.Add("serpNumberAdverts",serpNumberAdverts);
    p.Add("serpAds",serpAds);
    p.Add("serpTitleNoWhat",serpTitleNoWhat);
    p.Add("serpDescriptionNoWhat",serpDescriptionNoWhat);
    p.Add("cookiePolicyUrl",cookiePolicyUrl);
    p.Add("cookiePolicyNotice",cookiePolicyNotice);
    p.Add("addBusinessButtonText",addBusinessButtonText);
    p.Add("twitterUrl",twitterUrl);
    p.Add("facebookUrl",facebookUrl);
    p.Add("copyright",copyright);
    p.Add("advertUpgradeActive",advertUpgradeActive);
    p.Add("advertUpgradePrice",advertUpgradePrice);
    p.Add("advertUpgradeMaxTags",advertUpgradeMaxTags);
    p.Add("advertUpgradeMaxLocations",advertUpgradeMaxLocations);
    p.Add("advertUpgradeContractLength",advertUpgradeContractLength);
    p.Add("advertUpgradeRefId",advertUpgradeRefId);
    p.Add("phoneReveal",phoneReveal);
    p.Add("loginLinkText",loginLinkText);
    p.Add("contextLocationId",contextLocationId);
    p.Add("addBusinessButtonPosition",addBusinessButtonPosition);
    p.Add("denyIndexing",denyIndexing);
    p.Add("contextRadius",contextRadius);
    p.Add("activityStream",activityStream);
    p.Add("activityStreamSize",activityStreamSize);
    return doCurl("POST","/flatpack",p);
  }


  /**
   * Upload a CSS file for the Admin for this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String postFlatpackAdminCSS( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/adminCSS",p);
  }


  /**
   * Upload an image to serve out as the large logo in the Admin for this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String postFlatpackAdminLargeLogo( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/adminLargeLogo",p);
  }


  /**
   * Upload an image to serve out as the small logo in the Admin for this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String postFlatpackAdminSmallLogo( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/adminSmallLogo",p);
  }


  /**
   * Get a flatpack using a domain name
   *
   *  @param domainName - the domain name to search for
   *  @return - the data from the api
  */
  public String getFlatpackBy_domain_name( String domainName) {
    Hashtable p = new Hashtable();
    p.Add("domainName",domainName);
    return doCurl("GET","/flatpack/by_domain_name",p);
  }


  /**
   * Get flatpacks that match the supplied masheryid
   *
   *  @return - the data from the api
  */
  public String getFlatpackBy_masheryid() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/flatpack/by_masheryid",p);
  }


  /**
   * Clone an existing flatpack
   *
   *  @param flatpack_id - the flatpack_id to clone
   *  @param domainName - the domain of the new flatpack site (no leading http:// or anything please)
   *  @return - the data from the api
  */
  public String getFlatpackClone( String flatpack_id, String domainName) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("domainName",domainName);
    return doCurl("GET","/flatpack/clone",p);
  }


  /**
   * Upload an icon to serve out with this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String postFlatpackIcon( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/icon",p);
  }


  /**
   * Remove a canned link to an existing flatpack site.
   *
   *  @param flatpack_id - the id of the flatpack to delete
   *  @param gen_id - the id of the canned link to remove
   *  @return - the data from the api
  */
  public String deleteFlatpackLink( String flatpack_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/flatpack/link",p);
  }


  /**
   * Add a canned link to an existing flatpack site.
   *
   *  @param flatpack_id - the id of the flatpack to delete
   *  @param keywords - the keywords to use in the canned search
   *  @param location - the location to use in the canned search
   *  @param linkText - the link text to be used to in the canned search link
   *  @return - the data from the api
  */
  public String postFlatpackLink( String flatpack_id, String keywords, String location, String linkText) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("keywords",keywords);
    p.Add("location",location);
    p.Add("linkText",linkText);
    return doCurl("POST","/flatpack/link",p);
  }


  /**
   * Upload a logo to serve out with this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String postFlatpackLogo( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/logo",p);
  }


  /**
   * Upload a TXT file to act as the sitemap for this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String postFlatpackSitemap( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/sitemap",p);
  }


  /**
   * Upload a file to our asset server and return the url
   *
   *  @param filedata
   *  @return - the data from the api
  */
  public String postFlatpackUpload( String filedata) {
    Hashtable p = new Hashtable();
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/upload",p);
  }


  /**
   * Returns group that matches a given group id
   *
   *  @param group_id
   *  @return - the data from the api
  */
  public String getGroup( String group_id) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    return doCurl("GET","/group",p);
  }


  /**
   * Update/Add a Group
   *
   *  @param group_id
   *  @param name
   *  @param description
   *  @param url
   *  @return - the data from the api
  */
  public String postGroup( String group_id, String name, String description, String url) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    p.Add("name",name);
    p.Add("description",description);
    p.Add("url",url);
    return doCurl("POST","/group",p);
  }


  /**
   * Delete a group with a specified group_id
   *
   *  @param group_id
   *  @return - the data from the api
  */
  public String deleteGroup( String group_id) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    return doCurl("DELETE","/group",p);
  }


  /**
   * Returns all groups
   *
   *  @return - the data from the api
  */
  public String getGroupAll() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/group/all",p);
  }


  /**
   * Bulk delete entities from a specified group
   *
   *  @param group_id
   *  @param filedata
   *  @return - the data from the api
  */
  public String postGroupBulk_delete( String group_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/group/bulk_delete",p);
  }


  /**
   * Bulk ingest entities into a specified group
   *
   *  @param group_id
   *  @param filedata
   *  @param category_type
   *  @return - the data from the api
  */
  public String postGroupBulk_ingest( String group_id, String filedata, String category_type) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    p.Add("filedata",filedata);
    p.Add("category_type",category_type);
    return doCurl("POST","/group/bulk_ingest",p);
  }


  /**
   * Bulk update entities with a specified group
   *
   *  @param group_id
   *  @param data
   *  @return - the data from the api
  */
  public String postGroupBulk_update( String group_id, String data) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    p.Add("data",data);
    return doCurl("POST","/group/bulk_update",p);
  }


  /**
   * Get number of claims today
   *
   *  @param from_date
   *  @param to_date
   *  @param country_id
   *  @return - the data from the api
  */
  public String getHeartbeatBy_date( String from_date, String to_date, String country_id) {
    Hashtable p = new Hashtable();
    p.Add("from_date",from_date);
    p.Add("to_date",to_date);
    p.Add("country_id",country_id);
    return doCurl("GET","/heartbeat/by_date",p);
  }


  /**
   * Get number of claims today
   *
   *  @param country
   *  @param claim_type
   *  @return - the data from the api
  */
  public String getHeartbeatTodayClaims( String country, String claim_type) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    p.Add("claim_type",claim_type);
    return doCurl("GET","/heartbeat/today/claims",p);
  }


  /**
   * Process a bulk file
   *
   *  @param job_id
   *  @param filedata - A tab separated file for ingest
   *  @return - the data from the api
  */
  public String postIngest_file( String job_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("job_id",job_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/ingest_file",p);
  }


  /**
   * Get an ingest job from the collection
   *
   *  @param job_id
   *  @return - the data from the api
  */
  public String getIngest_job( String job_id) {
    Hashtable p = new Hashtable();
    p.Add("job_id",job_id);
    return doCurl("GET","/ingest_job",p);
  }


  /**
   * Add a ingest job to the collection
   *
   *  @param description
   *  @param category_type
   *  @return - the data from the api
  */
  public String postIngest_job( String description, String category_type) {
    Hashtable p = new Hashtable();
    p.Add("description",description);
    p.Add("category_type",category_type);
    return doCurl("POST","/ingest_job",p);
  }


  /**
   * Get an ingest log from the collection
   *
   *  @param job_id
   *  @param success
   *  @param errors
   *  @param limit
   *  @param skip
   *  @return - the data from the api
  */
  public String getIngest_logBy_job_id( String job_id, String success, String errors, String limit, String skip) {
    Hashtable p = new Hashtable();
    p.Add("job_id",job_id);
    p.Add("success",success);
    p.Add("errors",errors);
    p.Add("limit",limit);
    p.Add("skip",skip);
    return doCurl("GET","/ingest_log/by_job_id",p);
  }


  /**
   * Check the status of the Ingest queue, and potentially flush it
   *
   *  @param flush
   *  @return - the data from the api
  */
  public String getIngest_queue( String flush) {
    Hashtable p = new Hashtable();
    p.Add("flush",flush);
    return doCurl("GET","/ingest_queue",p);
  }


  /**
   * Create/update a new locz document with the supplied ID in the locations reference database.
   *
   *  @param location_id
   *  @param type
   *  @param country
   *  @param language
   *  @param name
   *  @param formal_name
   *  @param resolution
   *  @param population
   *  @param description
   *  @param timezone
   *  @param latitude
   *  @param longitude
   *  @param parent_town
   *  @param parent_county
   *  @param parent_province
   *  @param parent_region
   *  @param parent_neighbourhood
   *  @param parent_district
   *  @param postalcode
   *  @param searchable_id
   *  @param searchable_ids
   *  @return - the data from the api
  */
  public String postLocation( String location_id, String type, String country, String language, String name, String formal_name, String resolution, String population, String description, String timezone, String latitude, String longitude, String parent_town, String parent_county, String parent_province, String parent_region, String parent_neighbourhood, String parent_district, String postalcode, String searchable_id, String searchable_ids) {
    Hashtable p = new Hashtable();
    p.Add("location_id",location_id);
    p.Add("type",type);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("name",name);
    p.Add("formal_name",formal_name);
    p.Add("resolution",resolution);
    p.Add("population",population);
    p.Add("description",description);
    p.Add("timezone",timezone);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("parent_town",parent_town);
    p.Add("parent_county",parent_county);
    p.Add("parent_province",parent_province);
    p.Add("parent_region",parent_region);
    p.Add("parent_neighbourhood",parent_neighbourhood);
    p.Add("parent_district",parent_district);
    p.Add("postalcode",postalcode);
    p.Add("searchable_id",searchable_id);
    p.Add("searchable_ids",searchable_ids);
    return doCurl("POST","/location",p);
  }


  /**
   * Read a location with the supplied ID in the locations reference database.
   *
   *  @param location_id
   *  @return - the data from the api
  */
  public String getLocation( String location_id) {
    Hashtable p = new Hashtable();
    p.Add("location_id",location_id);
    return doCurl("GET","/location",p);
  }


  /**
   * Read multiple locations with the supplied ID in the locations reference database.
   *
   *  @param location_ids
   *  @return - the data from the api
  */
  public String getLocationMultiple( String location_ids) {
    Hashtable p = new Hashtable();
    p.Add("location_ids",location_ids);
    return doCurl("GET","/location/multiple",p);
  }


  /**
   * Fetch the project logo, the symbol of the Wolf
   *
   *  @param a
   *  @param b
   *  @param c
   *  @param d
   *  @return - the data from the api
  */
  public String getLogo( String a, String b, String c, String d) {
    Hashtable p = new Hashtable();
    p.Add("a",a);
    p.Add("b",b);
    p.Add("c",c);
    p.Add("d",d);
    return doCurl("GET","/logo",p);
  }


  /**
   * Fetch the project logo, the symbol of the Wolf
   *
   *  @param a
   *  @return - the data from the api
  */
  public String putLogo( String a) {
    Hashtable p = new Hashtable();
    p.Add("a",a);
    return doCurl("PUT","/logo",p);
  }


  /**
   * Find a category from cache or cloudant depending if it is in the cache
   *
   *  @param string - A string to search against, E.g. Plumbers
   *  @param language - An ISO compatible language code, E.g. en
   *  @return - the data from the api
  */
  public String getLookupCategory( String _string, String language) {
    Hashtable p = new Hashtable();
    p.Add("string",_string);
    p.Add("language",language);
    return doCurl("GET","/lookup/category",p);
  }


  /**
   * Find a category from a legacy ID or supplier (e.g. bill_moss)
   *
   *  @param id
   *  @param type
   *  @return - the data from the api
  */
  public String getLookupLegacyCategory( String id, String type) {
    Hashtable p = new Hashtable();
    p.Add("id",id);
    p.Add("type",type);
    return doCurl("GET","/lookup/legacy/category",p);
  }


  /**
   * Find a location from cache or cloudant depending if it is in the cache (locz)
   *
   *  @param string
   *  @param language
   *  @param country
   *  @param latitude
   *  @param longitude
   *  @return - the data from the api
  */
  public String getLookupLocation( String _string, String language, String country, String latitude, String longitude) {
    Hashtable p = new Hashtable();
    p.Add("string",_string);
    p.Add("language",language);
    p.Add("country",country);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    return doCurl("GET","/lookup/location",p);
  }


  /**
   * Find all matches by phone number and then return all matches that also match company name and location. Default location_strictness is defined in Km and the default is set to 0.2 (200m)
   *
   *  @param phone
   *  @param company_name
   *  @param latitude
   *  @param longitude
   *  @param postcode
   *  @param country
   *  @param name_strictness
   *  @param location_strictness
   *  @return - the data from the api
  */
  public String getMatchByphone( String phone, String company_name, String latitude, String longitude, String postcode, String country, String name_strictness, String location_strictness) {
    Hashtable p = new Hashtable();
    p.Add("phone",phone);
    p.Add("company_name",company_name);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("postcode",postcode);
    p.Add("country",country);
    p.Add("name_strictness",name_strictness);
    p.Add("location_strictness",location_strictness);
    return doCurl("GET","/match/byphone",p);
  }


  /**
   * Find all matches by phone number, returning up to 10 matches
   *
   *  @param phone
   *  @param country
   *  @param exclude - Entity ID to exclude from the results
   *  @return - the data from the api
  */
  public String getMatchByphone2( String phone, String country, String exclude) {
    Hashtable p = new Hashtable();
    p.Add("phone",phone);
    p.Add("country",country);
    p.Add("exclude",exclude);
    return doCurl("GET","/match/byphone2",p);
  }


  /**
   * Perform a match on the two supplied entities, returning the outcome and showing our working
   *
   *  @param primary_entity_id
   *  @param secondary_entity_id
   *  @param return_entities - Should we return the entity documents
   *  @return - the data from the api
  */
  public String getMatchOftheday( String primary_entity_id, String secondary_entity_id, String return_entities) {
    Hashtable p = new Hashtable();
    p.Add("primary_entity_id",primary_entity_id);
    p.Add("secondary_entity_id",secondary_entity_id);
    p.Add("return_entities",return_entities);
    return doCurl("GET","/match/oftheday",p);
  }


  /**
   * Create a matching log
   *
   *  @param processed_entity_id
   *  @param matched_entity_id
   *  @param processed_mega
   *  @param matched_mega
   *  @param processed_group
   *  @param matched_group
   *  @param merged
   *  @return - the data from the api
  */
  public String putMatching_log( String processed_entity_id, String matched_entity_id, String processed_mega, String matched_mega, String processed_group, String matched_group, String merged) {
    Hashtable p = new Hashtable();
    p.Add("processed_entity_id",processed_entity_id);
    p.Add("matched_entity_id",matched_entity_id);
    p.Add("processed_mega",processed_mega);
    p.Add("matched_mega",matched_mega);
    p.Add("processed_group",processed_group);
    p.Add("matched_group",matched_group);
    p.Add("merged",merged);
    return doCurl("PUT","/matching_log",p);
  }


  /**
   * Update/Add a message
   *
   *  @param message_id - Message id to pull
   *  @param ses_id - Aamazon email id
   *  @param from_user_id - User sending the message
   *  @param from_email - Sent from email address
   *  @param to_entity_id - The id of the entity being sent the message
   *  @param to_email - Sent from email address
   *  @param subject - Subject for the message
   *  @param body - Body for the message
   *  @param bounced - If the message bounced
   *  @return - the data from the api
  */
  public String postMessage( String message_id, String ses_id, String from_user_id, String from_email, String to_entity_id, String to_email, String subject, String body, String bounced) {
    Hashtable p = new Hashtable();
    p.Add("message_id",message_id);
    p.Add("ses_id",ses_id);
    p.Add("from_user_id",from_user_id);
    p.Add("from_email",from_email);
    p.Add("to_entity_id",to_entity_id);
    p.Add("to_email",to_email);
    p.Add("subject",subject);
    p.Add("body",body);
    p.Add("bounced",bounced);
    return doCurl("POST","/message",p);
  }


  /**
   * Fetching a message
   *
   *  @param message_id - The message id to pull the message for
   *  @return - the data from the api
  */
  public String getMessage( String message_id) {
    Hashtable p = new Hashtable();
    p.Add("message_id",message_id);
    return doCurl("GET","/message",p);
  }


  /**
   * Fetching messages by ses_id
   *
   *  @param ses_id - The amazon id to pull the message for
   *  @return - the data from the api
  */
  public String getMessageBy_ses_id( String ses_id) {
    Hashtable p = new Hashtable();
    p.Add("ses_id",ses_id);
    return doCurl("GET","/message/by_ses_id",p);
  }


  /**
   * With a known entity id, a private object can be added.
   *
   *  @param entity_id - The entity to associate the private object with
   *  @param data - The data to store
   *  @return - the data from the api
  */
  public String putPrivate_object( String entity_id, String data) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("data",data);
    return doCurl("PUT","/private_object",p);
  }


  /**
   * Allows a private object to be removed
   *
   *  @param private_object_id - The id of the private object to remove
   *  @return - the data from the api
  */
  public String deletePrivate_object( String private_object_id) {
    Hashtable p = new Hashtable();
    p.Add("private_object_id",private_object_id);
    return doCurl("DELETE","/private_object",p);
  }


  /**
   * Allows a private object to be returned based on the entity_id and masheryid
   *
   *  @param entity_id - The entity associated with the private object
   *  @return - the data from the api
  */
  public String getPrivate_objectAll( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/private_object/all",p);
  }


  /**
   * Update/Add a product
   *
   *  @param product_id - The ID of the product
   *  @param name - The name of the product
   *  @param strapline - The description of the product
   *  @param price - The price of the product
   *  @param currency - The currency in which the price is in
   *  @param active - is this an active product
   *  @param billing_period
   *  @return - the data from the api
  */
  public String postProduct( String product_id, String name, String strapline, String price, String currency, String active, String billing_period) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("name",name);
    p.Add("strapline",strapline);
    p.Add("price",price);
    p.Add("currency",currency);
    p.Add("active",active);
    p.Add("billing_period",billing_period);
    return doCurl("POST","/product",p);
  }


  /**
   * Returns the product information given a valid product_id
   *
   *  @param product_id
   *  @return - the data from the api
  */
  public String getProduct( String product_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    return doCurl("GET","/product",p);
  }


  /**
   * Removes a provisioning object from product
   *
   *  @param product_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String deleteProductProvisioning( String product_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/product/provisioning",p);
  }


  /**
   * Adds advertising provisioning object to a product
   *
   *  @param product_id
   *  @param publisher_id
   *  @param max_tags
   *  @param max_locations
   *  @return - the data from the api
  */
  public String postProductProvisioningAdvert( String product_id, String publisher_id, String max_tags, String max_locations) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("publisher_id",publisher_id);
    p.Add("max_tags",max_tags);
    p.Add("max_locations",max_locations);
    return doCurl("POST","/product/provisioning/advert",p);
  }


  /**
   * Adds claim provisioning object to a product
   *
   *  @param product_id
   *  @return - the data from the api
  */
  public String postProductProvisioningClaim( String product_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    return doCurl("POST","/product/provisioning/claim",p);
  }


  /**
   * Adds syndication provisioning object to a product
   *
   *  @param product_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String postProductProvisioningSyndication( String product_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("POST","/product/provisioning/syndication",p);
  }


  /**
   * Perform the whole PTB process on the supplied entity
   *
   *  @param entity_id
   *  @param destructive
   *  @return - the data from the api
  */
  public String getPtbAll( String entity_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("destructive",destructive);
    return doCurl("GET","/ptb/all",p);
  }


  /**
   * Report on what happened to specific entity_id
   *
   *  @param year - the year to examine
   *  @param month - the month to examine
   *  @param entity_id - the entity to research
   *  @return - the data from the api
  */
  public String getPtbLog( String year, String month, String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("year",year);
    p.Add("month",month);
    p.Add("entity_id",entity_id);
    return doCurl("GET","/ptb/log",p);
  }


  /**
   * Process an entity with a specific PTB module
   *
   *  @param entity_id
   *  @param module
   *  @param destructive
   *  @return - the data from the api
  */
  public String getPtbModule( String entity_id, String module, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("module",module);
    p.Add("destructive",destructive);
    return doCurl("GET","/ptb/module",p);
  }


  /**
   * Report on the run-rate of the Paint the Bridge System
   *
   *  @param country - the country to get the runrate for
   *  @param year - the year to examine
   *  @param month - the month to examine
   *  @param day - the day to examine
   *  @return - the data from the api
  */
  public String getPtbRunrate( String country, String year, String month, String day) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    p.Add("year",year);
    p.Add("month",month);
    p.Add("day",day);
    return doCurl("GET","/ptb/runrate",p);
  }


  /**
   * Report on the value being added by Paint The Bridge
   *
   *  @param country - the country to get the runrate for
   *  @param year - the year to examine
   *  @param month - the month to examine
   *  @param day - the day to examine
   *  @return - the data from the api
  */
  public String getPtbValueadded( String country, String year, String month, String day) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    p.Add("year",year);
    p.Add("month",month);
    p.Add("day",day);
    return doCurl("GET","/ptb/valueadded",p);
  }


  /**
   * Delete a publisher with a specified publisher_id
   *
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String deletePublisher( String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("publisher_id",publisher_id);
    return doCurl("DELETE","/publisher",p);
  }


  /**
   * Update/Add a publisher
   *
   *  @param publisher_id
   *  @param country
   *  @param name
   *  @param description
   *  @param active
   *  @return - the data from the api
  */
  public String postPublisher( String publisher_id, String country, String name, String description, String active) {
    Hashtable p = new Hashtable();
    p.Add("publisher_id",publisher_id);
    p.Add("country",country);
    p.Add("name",name);
    p.Add("description",description);
    p.Add("active",active);
    return doCurl("POST","/publisher",p);
  }


  /**
   * Returns publisher that matches a given publisher id
   *
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String getPublisher( String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("publisher_id",publisher_id);
    return doCurl("GET","/publisher",p);
  }


  /**
   * Returns publisher that matches a given publisher id
   *
   *  @param country
   *  @return - the data from the api
  */
  public String getPublisherByCountry( String country) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    return doCurl("GET","/publisher/byCountry",p);
  }


  /**
   * Returns publishers that are available for a given entity_id.
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String getPublisherByEntityId( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/publisher/byEntityId",p);
  }


  /**
   * Create a queue item
   *
   *  @param queue_name
   *  @param data
   *  @return - the data from the api
  */
  public String putQueue( String queue_name, String data) {
    Hashtable p = new Hashtable();
    p.Add("queue_name",queue_name);
    p.Add("data",data);
    return doCurl("PUT","/queue",p);
  }


  /**
   * With a known queue id, a queue item can be removed.
   *
   *  @param queue_id
   *  @return - the data from the api
  */
  public String deleteQueue( String queue_id) {
    Hashtable p = new Hashtable();
    p.Add("queue_id",queue_id);
    return doCurl("DELETE","/queue",p);
  }


  /**
   * Retrieve queue items.
   *
   *  @param limit
   *  @param queue_name
   *  @return - the data from the api
  */
  public String getQueue( String limit, String queue_name) {
    Hashtable p = new Hashtable();
    p.Add("limit",limit);
    p.Add("queue_name",queue_name);
    return doCurl("GET","/queue",p);
  }


  /**
   * Add an error to a queue item
   *
   *  @param queue_id
   *  @param error
   *  @return - the data from the api
  */
  public String postQueueError( String queue_id, String error) {
    Hashtable p = new Hashtable();
    p.Add("queue_id",queue_id);
    p.Add("error",error);
    return doCurl("POST","/queue/error",p);
  }


  /**
   * Find a queue item by its type and id
   *
   *  @param type
   *  @param id
   *  @return - the data from the api
  */
  public String getQueueSearch( String type, String id) {
    Hashtable p = new Hashtable();
    p.Add("type",type);
    p.Add("id",id);
    return doCurl("GET","/queue/search",p);
  }


  /**
   * Unlock queue items.
   *
   *  @param queue_name
   *  @param seconds
   *  @return - the data from the api
  */
  public String postQueueUnlock( String queue_name, String seconds) {
    Hashtable p = new Hashtable();
    p.Add("queue_name",queue_name);
    p.Add("seconds",seconds);
    return doCurl("POST","/queue/unlock",p);
  }


  /**
   * Update/Add a reseller
   *
   *  @param reseller_id
   *  @param country
   *  @param name
   *  @param description
   *  @param active
   *  @return - the data from the api
  */
  public String postReseller( String reseller_id, String country, String name, String description, String active) {
    Hashtable p = new Hashtable();
    p.Add("reseller_id",reseller_id);
    p.Add("country",country);
    p.Add("name",name);
    p.Add("description",description);
    p.Add("active",active);
    return doCurl("POST","/reseller",p);
  }


  /**
   * Returns reseller that matches a given reseller id
   *
   *  @param reseller_id
   *  @return - the data from the api
  */
  public String getReseller( String reseller_id) {
    Hashtable p = new Hashtable();
    p.Add("reseller_id",reseller_id);
    return doCurl("GET","/reseller",p);
  }


  /**
   * Return a sales log by id
   *
   *  @param sales_log_id - The sales log id to pull
   *  @return - the data from the api
  */
  public String getSales_log( String sales_log_id) {
    Hashtable p = new Hashtable();
    p.Add("sales_log_id",sales_log_id);
    return doCurl("GET","/sales_log",p);
  }


  /**
   * Return a sales log by id
   *
   *  @param from_date
   *  @param country
   *  @param action_type
   *  @return - the data from the api
  */
  public String getSales_logBy_countryBy_date( String from_date, String country, String action_type) {
    Hashtable p = new Hashtable();
    p.Add("from_date",from_date);
    p.Add("country",country);
    p.Add("action_type",action_type);
    return doCurl("GET","/sales_log/by_country/by_date",p);
  }


  /**
   * Return a sales log by id
   *
   *  @param from_date
   *  @param to_date
   *  @return - the data from the api
  */
  public String getSales_logBy_date( String from_date, String to_date) {
    Hashtable p = new Hashtable();
    p.Add("from_date",from_date);
    p.Add("to_date",to_date);
    return doCurl("GET","/sales_log/by_date",p);
  }


  /**
   * Log a sale
   *
   *  @param entity_id - The entity the sale was made against
   *  @param country - The country code the sales log orginated
   *  @param action_type - The type of action we are performing
   *  @param publisher_id - The publisher id that has made the sale
   *  @param mashery_id - The mashery id
   *  @param reseller_ref - The reference of the sale made by the seller
   *  @param reseller_agent_id - The id of the agent selling the product
   *  @param max_tags - The number of tags available to the entity
   *  @param max_locations - The number of locations available to the entity
   *  @param extra_tags - The extra number of tags
   *  @param extra_locations - The extra number of locations
   *  @param expiry_date - The date the product expires
   *  @return - the data from the api
  */
  public String postSales_logEntity( String entity_id, String country, String action_type, String publisher_id, String mashery_id, String reseller_ref, String reseller_agent_id, String max_tags, String max_locations, String extra_tags, String extra_locations, String expiry_date) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("country",country);
    p.Add("action_type",action_type);
    p.Add("publisher_id",publisher_id);
    p.Add("mashery_id",mashery_id);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_agent_id",reseller_agent_id);
    p.Add("max_tags",max_tags);
    p.Add("max_locations",max_locations);
    p.Add("extra_tags",extra_tags);
    p.Add("extra_locations",extra_locations);
    p.Add("expiry_date",expiry_date);
    return doCurl("POST","/sales_log/entity",p);
  }


  /**
   * Add a Sales Log document for a syndication action
   *
   *  @param action_type
   *  @param syndication_type
   *  @param publisher_id
   *  @param expiry_date
   *  @param entity_id
   *  @param group_id
   *  @param seed_masheryid
   *  @param supplier_masheryid
   *  @param country
   *  @param reseller_masheryid
   *  @return - the data from the api
  */
  public String postSales_logSyndication( String action_type, String syndication_type, String publisher_id, String expiry_date, String entity_id, String group_id, String seed_masheryid, String supplier_masheryid, String country, String reseller_masheryid) {
    Hashtable p = new Hashtable();
    p.Add("action_type",action_type);
    p.Add("syndication_type",syndication_type);
    p.Add("publisher_id",publisher_id);
    p.Add("expiry_date",expiry_date);
    p.Add("entity_id",entity_id);
    p.Add("group_id",group_id);
    p.Add("seed_masheryid",seed_masheryid);
    p.Add("supplier_masheryid",supplier_masheryid);
    p.Add("country",country);
    p.Add("reseller_masheryid",reseller_masheryid);
    return doCurl("POST","/sales_log/syndication",p);
  }


  /**
   * For insance, reporting a phone number as wrong
   *
   *  @param entity_id - A valid entity_id e.g. 379236608286720
   *  @param country - The country code from where the signal originated e.g. ie
   *  @param gen_id - The gen_id for the item being reported
   *  @param signal_type - The signal that is to be reported e.g. wrong
   *  @param data_type - The type of data being reported
   *  @param inactive_reason - The reason for making the entity inactive
   *  @param inactive_description - A description to accompany the inactive reasoning
   *  @return - the data from the api
  */
  public String postSignal( String entity_id, String country, String gen_id, String signal_type, String data_type, String inactive_reason, String inactive_description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("country",country);
    p.Add("gen_id",gen_id);
    p.Add("signal_type",signal_type);
    p.Add("data_type",data_type);
    p.Add("inactive_reason",inactive_reason);
    p.Add("inactive_description",inactive_description);
    return doCurl("POST","/signal",p);
  }


  /**
   * Get the number of times an entity has been served out as an advert or on serps/bdp pages
   *
   *  @param entity_id - A valid entity_id e.g. 379236608286720
   *  @param year - The year to report on
   *  @param month - The month to report on
   *  @return - the data from the api
  */
  public String getStatsEntityBy_date( String entity_id, String year, String month) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("year",year);
    p.Add("month",month);
    return doCurl("GET","/stats/entity/by_date",p);
  }


  /**
   * Get the stats on an entity in a given year
   *
   *  @param entity_id - A valid entity_id e.g. 379236608286720
   *  @param year - The year to report on
   *  @return - the data from the api
  */
  public String getStatsEntityBy_year( String entity_id, String year) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("year",year);
    return doCurl("GET","/stats/entity/by_year",p);
  }


  /**
   * Confirms that the API is active, and returns the current version number
   *
   *  @return - the data from the api
  */
  public String getStatus() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/status",p);
  }


  /**
   * get a Syndication
   *
   *  @param syndication_id
   *  @return - the data from the api
  */
  public String getSyndication( String syndication_id) {
    Hashtable p = new Hashtable();
    p.Add("syndication_id",syndication_id);
    return doCurl("GET","/syndication",p);
  }


  /**
   * Add a Syndicate
   *
   *  @param syndication_type
   *  @param publisher_id
   *  @param expiry_date
   *  @param entity_id
   *  @param group_id
   *  @param seed_masheryid
   *  @param supplier_masheryid
   *  @param country
   *  @return - the data from the api
  */
  public String postSyndicationCreate( String syndication_type, String publisher_id, String expiry_date, String entity_id, String group_id, String seed_masheryid, String supplier_masheryid, String country) {
    Hashtable p = new Hashtable();
    p.Add("syndication_type",syndication_type);
    p.Add("publisher_id",publisher_id);
    p.Add("expiry_date",expiry_date);
    p.Add("entity_id",entity_id);
    p.Add("group_id",group_id);
    p.Add("seed_masheryid",seed_masheryid);
    p.Add("supplier_masheryid",supplier_masheryid);
    p.Add("country",country);
    return doCurl("POST","/syndication/create",p);
  }


  /**
   * Renew a Syndicate
   *
   *  @param syndication_type
   *  @param publisher_id
   *  @param entity_id
   *  @param group_id
   *  @param seed_masheryid
   *  @param supplier_masheryid
   *  @param country
   *  @param reseller_masheryid - This parameter is derived from the incoming Mashery API key and is stored as reseller_masheryid in the syndication document.
   *  @param expiry_date
   *  @return - the data from the api
  */
  public String postSyndicationRenew( String syndication_type, String publisher_id, String entity_id, String group_id, String seed_masheryid, String supplier_masheryid, String country, String reseller_masheryid, String expiry_date) {
    Hashtable p = new Hashtable();
    p.Add("syndication_type",syndication_type);
    p.Add("publisher_id",publisher_id);
    p.Add("entity_id",entity_id);
    p.Add("group_id",group_id);
    p.Add("seed_masheryid",seed_masheryid);
    p.Add("supplier_masheryid",supplier_masheryid);
    p.Add("country",country);
    p.Add("reseller_masheryid",reseller_masheryid);
    p.Add("expiry_date",expiry_date);
    return doCurl("POST","/syndication/renew",p);
  }


  /**
   * When we get a syndication update make a record of it
   *
   *  @param entity_id - The entity to pull
   *  @param publisher_id - The publisher this log entry refers to
   *  @param action - The log type
   *  @param success - If the syndication was successful
   *  @param message - An optional message e.g. submitted to the syndication partner
   *  @param syndicated_id - The entity as known to the publisher
   *  @return - the data from the api
  */
  public String postSyndication_log( String entity_id, String publisher_id, String action, String success, String message, String syndicated_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("publisher_id",publisher_id);
    p.Add("action",action);
    p.Add("success",success);
    p.Add("message",message);
    p.Add("syndicated_id",syndicated_id);
    return doCurl("POST","/syndication_log",p);
  }


  /**
   * Get all syndication log entries for a given entity id
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String getSyndication_logBy_entity_id( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/syndication_log/by_entity_id",p);
  }


  /**
   * Provides a tokenised URL to redirect a user so they can add an entity to Central Index
   *
   *  @param language - The language to use to render the add path e.g. en
   *  @param portal_name - The name of the website that data is to be added on e.g. YourLocal
   *  @param country - The country of the entity to be added e.g. gb
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @return - the data from the api
  */
  public String getTokenAdd( String language, String portal_name, String country, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("language",language);
    p.Add("portal_name",portal_name);
    p.Add("country",country);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/add",p);
  }


  /**
   * Provides a tokenised URL to redirect a user to claim an entity on Central Index
   *
   *  @param entity_id - Entity ID to be claimed e.g. 380348266819584
   *  @param language - The language to use to render the claim path e.g. en
   *  @param portal_name - The name of the website that entity is being claimed on e.g. YourLocal
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @return - the data from the api
  */
  public String getTokenClaim( String entity_id, String language, String portal_name, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("language",language);
    p.Add("portal_name",portal_name);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/claim",p);
  }


  /**
   * Allows us to identify the user, entity and element from an encoded endpoint URL's token
   *
   *  @param token
   *  @return - the data from the api
  */
  public String getTokenDecode( String token) {
    Hashtable p = new Hashtable();
    p.Add("token",token);
    return doCurl("GET","/token/decode",p);
  }


  /**
   * Fetch token for edit path
   *
   *  @param entity_id - The id of the entity being upgraded
   *  @param language - The language for the app
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param edit_page - the page in the edit path that the user should land on
   *  @return - the data from the api
  */
  public String getTokenEdit( String entity_id, String language, String flatpack_id, String edit_page) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    p.Add("edit_page",edit_page);
    return doCurl("GET","/token/edit",p);
  }


  /**
   * Fetch token for login path
   *
   *  @param portal_name - The name of the application that has initiated the login process, example: 'Your Local'
   *  @param language - The language for the app
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @return - the data from the api
  */
  public String getTokenLogin( String portal_name, String language, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("portal_name",portal_name);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/login",p);
  }


  /**
   * Fetch token for messaging path
   *
   *  @param entity_id - The id of the entity being messaged
   *  @param portal_name - The name of the application that has initiated the email process, example: 'Your Local'
   *  @param language - The language for the app
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @return - the data from the api
  */
  public String getTokenMessage( String entity_id, String portal_name, String language, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("portal_name",portal_name);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/message",p);
  }


  /**
   * Fetch token for product path
   *
   *  @param entity_id - The id of the entity to add a product to
   *  @param product_id - The product id of the product
   *  @param language - The language for the app
   *  @param portal_name - The portal name
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param source - email, social media etc
   *  @param channel - 
   *  @param campaign - the campaign identifier
   *  @return - the data from the api
  */
  public String getTokenProduct( String entity_id, String product_id, String language, String portal_name, String flatpack_id, String source, String channel, String campaign) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("product_id",product_id);
    p.Add("language",language);
    p.Add("portal_name",portal_name);
    p.Add("flatpack_id",flatpack_id);
    p.Add("source",source);
    p.Add("channel",channel);
    p.Add("campaign",campaign);
    return doCurl("GET","/token/product",p);
  }


  /**
   * Provides a tokenised URL that allows a user to report incorrect entity information
   *
   *  @param entity_id - The unique Entity ID e.g. 379236608286720
   *  @param portal_name - The name of the portal that the user is coming from e.g. YourLocal
   *  @param language - The language to use to render the report path
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @return - the data from the api
  */
  public String getTokenReport( String entity_id, String portal_name, String language, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("portal_name",portal_name);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/report",p);
  }


  /**
   * Fetch token for update path
   *
   *  @param entity_id - The id of the entity being upgraded
   *  @param portal_name - The name of the application that has initiated the login process, example: 'Your Local'
   *  @param language - The language for the app
   *  @param price - The price of the advert in the entities native currency
   *  @param max_tags - The number of tags attached to the advert
   *  @param max_locations - The number of locations attached to the advert
   *  @param contract_length - The number of days from the initial sale date that the contract is valid for
   *  @param ref_id - The campaign or reference id
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @return - the data from the api
  */
  public String getTokenUpgrade( String entity_id, String portal_name, String language, String price, String max_tags, String max_locations, String contract_length, String ref_id, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("portal_name",portal_name);
    p.Add("language",language);
    p.Add("price",price);
    p.Add("max_tags",max_tags);
    p.Add("max_locations",max_locations);
    p.Add("contract_length",contract_length);
    p.Add("ref_id",ref_id);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/upgrade",p);
  }


  /**
   * The JaroWinklerDistance between two entities postal address objects
   *
   *  @param first_entity_id - The entity id of the first entity to be used in the postal address difference
   *  @param second_entity_id - The entity id of the second entity to be used in the postal address difference
   *  @return - the data from the api
  */
  public String getToolsAddressdiff( String first_entity_id, String second_entity_id) {
    Hashtable p = new Hashtable();
    p.Add("first_entity_id",first_entity_id);
    p.Add("second_entity_id",second_entity_id);
    return doCurl("GET","/tools/addressdiff",p);
  }


  /**
   * Use this call to get information (in HTML or JSON) about the data structure of given entity object (e.g. a phone number or an address)
   *
   *  @param object - The API call documentation is required for
   *  @param format - The format of the returned data eg. JSON or HTML
   *  @return - the data from the api
  */
  public String getToolsDocs( String _object, String format) {
    Hashtable p = new Hashtable();
    p.Add("object",_object);
    p.Add("format",format);
    return doCurl("GET","/tools/docs",p);
  }


  /**
   * Format an address according to the rules of the country supplied
   *
   *  @param address - The address to format
   *  @param country - The country where the address is based
   *  @return - the data from the api
  */
  public String getToolsFormatAddress( String address, String country) {
    Hashtable p = new Hashtable();
    p.Add("address",address);
    p.Add("country",country);
    return doCurl("GET","/tools/format/address",p);
  }


  /**
   * Format a phone number according to the rules of the country supplied
   *
   *  @param number - The telephone number to format
   *  @param country - The country where the telephone number is based
   *  @return - the data from the api
  */
  public String getToolsFormatPhone( String number, String country) {
    Hashtable p = new Hashtable();
    p.Add("number",number);
    p.Add("country",country);
    return doCurl("GET","/tools/format/phone",p);
  }


  /**
   * Supply an address to geocode - returns lat/lon and accuracy
   *
   *  @param building_number
   *  @param address1
   *  @param address2
   *  @param address3
   *  @param district
   *  @param town
   *  @param county
   *  @param province
   *  @param postcode
   *  @param country
   *  @return - the data from the api
  */
  public String getToolsGeocode( String building_number, String address1, String address2, String address3, String district, String town, String county, String province, String postcode, String country) {
    Hashtable p = new Hashtable();
    p.Add("building_number",building_number);
    p.Add("address1",address1);
    p.Add("address2",address2);
    p.Add("address3",address3);
    p.Add("district",district);
    p.Add("town",town);
    p.Add("county",county);
    p.Add("province",province);
    p.Add("postcode",postcode);
    p.Add("country",country);
    return doCurl("GET","/tools/geocode",p);
  }


  /**
   * Given a spreadsheet id add a row
   *
   *  @param spreadsheet_key - The key of the spreadsheet to edit
   *  @param data - A comma separated list to add as cells
   *  @return - the data from the api
  */
  public String postToolsGooglesheetAdd_row( String spreadsheet_key, String data) {
    Hashtable p = new Hashtable();
    p.Add("spreadsheet_key",spreadsheet_key);
    p.Add("data",data);
    return doCurl("POST","/tools/googlesheet/add_row",p);
  }


  /**
   * Given some image data we can resize and upload the images
   *
   *  @param filedata - The image data to upload and resize
   *  @param type - The type of image to upload and resize
   *  @return - the data from the api
  */
  public String postToolsImage( String filedata, String type) {
    Hashtable p = new Hashtable();
    p.Add("filedata",filedata);
    p.Add("type",type);
    return doCurl("POST","/tools/image",p);
  }


  /**
   * Generate JSON in the format to generate Mashery's IODocs
   *
   *  @param mode - The HTTP method of the API call to document. e.g. GET
   *  @param path - The path of the API call to document e.g, /entity
   *  @param endpoint - The Mashery 'endpoint' to prefix to our API paths e.g. v1
   *  @param doctype - Mashery has two forms of JSON to describe API methods; one on github, the other on its customer dashboard
   *  @return - the data from the api
  */
  public String getToolsIodocs( String mode, String path, String endpoint, String doctype) {
    Hashtable p = new Hashtable();
    p.Add("mode",mode);
    p.Add("path",path);
    p.Add("endpoint",endpoint);
    p.Add("doctype",doctype);
    return doCurl("GET","/tools/iodocs",p);
  }


  /**
   * compile the supplied less with the standard Bootstrap less into a CSS file
   *
   *  @param less - The LESS code to compile
   *  @return - the data from the api
  */
  public String getToolsLess( String less) {
    Hashtable p = new Hashtable();
    p.Add("less",less);
    return doCurl("GET","/tools/less",p);
  }


  /**
   * Ring the person and verify their account
   *
   *  @param to - The phone number to verify
   *  @param from - The phone number to call from
   *  @param pin - The pin to verify the phone number with
   *  @param twilio_voice - The language to read the verification in
   *  @return - the data from the api
  */
  public String getToolsPhonecallVerify( String to, String from, String pin, String twilio_voice) {
    Hashtable p = new Hashtable();
    p.Add("to",to);
    p.Add("from",from);
    p.Add("pin",pin);
    p.Add("twilio_voice",twilio_voice);
    return doCurl("GET","/tools/phonecall/verify",p);
  }


  /**
   * Return the phonetic representation of a string
   *
   *  @param text
   *  @return - the data from the api
  */
  public String getToolsPhonetic( String text) {
    Hashtable p = new Hashtable();
    p.Add("text",text);
    return doCurl("GET","/tools/phonetic",p);
  }


  /**
   * Attempt to process a phone number, removing anything which is not a digit
   *
   *  @param number
   *  @return - the data from the api
  */
  public String getToolsProcess_phone( String number) {
    Hashtable p = new Hashtable();
    p.Add("number",number);
    return doCurl("GET","/tools/process_phone",p);
  }


  /**
   * Fully process a string. This includes removing punctuation, stops words and stemming a string. Also returns the phonetic representation of this string.
   *
   *  @param text
   *  @return - the data from the api
  */
  public String getToolsProcess_string( String text) {
    Hashtable p = new Hashtable();
    p.Add("text",text);
    return doCurl("GET","/tools/process_string",p);
  }


  /**
   * Force refresh of search indexes
   *
   *  @return - the data from the api
  */
  public String getToolsReindex() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/tools/reindex",p);
  }


  /**
   * replace some text parameters with some entity details
   *
   *  @param entity_id - The entity to pull for replacements
   *  @param string - The string full of parameters
   *  @return - the data from the api
  */
  public String getToolsReplace( String entity_id, String _string) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("string",_string);
    return doCurl("GET","/tools/replace",p);
  }


  /**
   * Check to see if a supplied email address is valid
   *
   *  @param from - The phone number from which the SMS orginates
   *  @param to - The phone number to which the SMS is to be sent
   *  @param message - The message to be sent in the SMS
   *  @return - the data from the api
  */
  public String getToolsSendsms( String from, String to, String message) {
    Hashtable p = new Hashtable();
    p.Add("from",from);
    p.Add("to",to);
    p.Add("message",message);
    return doCurl("GET","/tools/sendsms",p);
  }


  /**
   * Spider a single url looking for key facts
   *
   *  @param url
   *  @param pages
   *  @param country
   *  @return - the data from the api
  */
  public String getToolsSpider( String url, String pages, String country) {
    Hashtable p = new Hashtable();
    p.Add("url",url);
    p.Add("pages",pages);
    p.Add("country",country);
    return doCurl("GET","/tools/spider",p);
  }


  /**
   * Returns a stemmed version of a string
   *
   *  @param text
   *  @return - the data from the api
  */
  public String getToolsStem( String text) {
    Hashtable p = new Hashtable();
    p.Add("text",text);
    return doCurl("GET","/tools/stem",p);
  }


  /**
   * Removes stopwords from a string
   *
   *  @param text
   *  @return - the data from the api
  */
  public String getToolsStopwords( String text) {
    Hashtable p = new Hashtable();
    p.Add("text",text);
    return doCurl("GET","/tools/stopwords",p);
  }


  /**
   * Fetch the entity and convert it to DnB TSV format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String getToolsSyndicateDnb( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/dnb",p);
  }


  /**
   * Fetch the entity and convert it to TomTom XML format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String getToolsSyndicateGoogle( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/google",p);
  }


  /**
   * Fetch the entity and convert it to Google KML format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String getToolsSyndicateKml( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/kml",p);
  }


  /**
   * Fetch the entity and convert it to Nokia CSV format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String getToolsSyndicateNokia( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/nokia",p);
  }


  /**
   * Fetch the entity and convert it to TomTom XML format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String getToolsSyndicateTomtom( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/tomtom",p);
  }


  /**
   * Find a canonical URL from a supplied URL
   *
   *  @param url - The url to process
   *  @param max_redirects - The maximum number of reirects
   *  @return - the data from the api
  */
  public String getToolsUrl_details( String url, String max_redirects) {
    Hashtable p = new Hashtable();
    p.Add("url",url);
    p.Add("max_redirects",max_redirects);
    return doCurl("GET","/tools/url_details",p);
  }


  /**
   * Check to see if a supplied email address is valid
   *
   *  @param email_address - The email address to validate
   *  @return - the data from the api
  */
  public String getToolsValidate_email( String email_address) {
    Hashtable p = new Hashtable();
    p.Add("email_address",email_address);
    return doCurl("GET","/tools/validate_email",p);
  }


  /**
   * Calls a number to make sure it is active
   *
   *  @param phone_number - The phone number to validate
   *  @param country - The country code of the phone number to be validated
   *  @return - the data from the api
  */
  public String getToolsValidate_phone( String phone_number, String country) {
    Hashtable p = new Hashtable();
    p.Add("phone_number",phone_number);
    p.Add("country",country);
    return doCurl("GET","/tools/validate_phone",p);
  }


  /**
   * Update/Add a traction
   *
   *  @param traction_id
   *  @param trigger_type
   *  @param action_type
   *  @param country
   *  @param email_addresses
   *  @param title
   *  @param body
   *  @param api_method
   *  @param api_url
   *  @param api_params
   *  @param active
   *  @param reseller_masheryid
   *  @param publisher_masheryid
   *  @param description
   *  @return - the data from the api
  */
  public String postTraction( String traction_id, String trigger_type, String action_type, String country, String email_addresses, String title, String body, String api_method, String api_url, String api_params, String active, String reseller_masheryid, String publisher_masheryid, String description) {
    Hashtable p = new Hashtable();
    p.Add("traction_id",traction_id);
    p.Add("trigger_type",trigger_type);
    p.Add("action_type",action_type);
    p.Add("country",country);
    p.Add("email_addresses",email_addresses);
    p.Add("title",title);
    p.Add("body",body);
    p.Add("api_method",api_method);
    p.Add("api_url",api_url);
    p.Add("api_params",api_params);
    p.Add("active",active);
    p.Add("reseller_masheryid",reseller_masheryid);
    p.Add("publisher_masheryid",publisher_masheryid);
    p.Add("description",description);
    return doCurl("POST","/traction",p);
  }


  /**
   * Fetching a traction
   *
   *  @param traction_id
   *  @return - the data from the api
  */
  public String getTraction( String traction_id) {
    Hashtable p = new Hashtable();
    p.Add("traction_id",traction_id);
    return doCurl("GET","/traction",p);
  }


  /**
   * Deleting a traction
   *
   *  @param traction_id
   *  @return - the data from the api
  */
  public String deleteTraction( String traction_id) {
    Hashtable p = new Hashtable();
    p.Add("traction_id",traction_id);
    return doCurl("DELETE","/traction",p);
  }


  /**
   * Fetching active tractions
   *
   *  @return - the data from the api
  */
  public String getTractionActive() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/traction/active",p);
  }


  /**
   * Create a new transaction
   *
   *  @param entity_id
   *  @param user_id
   *  @param basket_total
   *  @param basket
   *  @param currency
   *  @param notes
   *  @return - the data from the api
  */
  public String putTransaction( String entity_id, String user_id, String basket_total, String basket, String currency, String notes) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("user_id",user_id);
    p.Add("basket_total",basket_total);
    p.Add("basket",basket);
    p.Add("currency",currency);
    p.Add("notes",notes);
    return doCurl("PUT","/transaction",p);
  }


  /**
   * Given a transaction_id retrieve information on it
   *
   *  @param transaction_id
   *  @return - the data from the api
  */
  public String getTransaction( String transaction_id) {
    Hashtable p = new Hashtable();
    p.Add("transaction_id",transaction_id);
    return doCurl("GET","/transaction",p);
  }


  /**
   * Set a transactions status to authorised
   *
   *  @param transaction_id
   *  @param paypal_getexpresscheckoutdetails
   *  @return - the data from the api
  */
  public String postTransactionAuthorised( String transaction_id, String paypal_getexpresscheckoutdetails) {
    Hashtable p = new Hashtable();
    p.Add("transaction_id",transaction_id);
    p.Add("paypal_getexpresscheckoutdetails",paypal_getexpresscheckoutdetails);
    return doCurl("POST","/transaction/authorised",p);
  }


  /**
   * Given a transaction_id retrieve information on it
   *
   *  @param paypal_transaction_id
   *  @return - the data from the api
  */
  public String getTransactionBy_paypal_transaction_id( String paypal_transaction_id) {
    Hashtable p = new Hashtable();
    p.Add("paypal_transaction_id",paypal_transaction_id);
    return doCurl("GET","/transaction/by_paypal_transaction_id",p);
  }


  /**
   * Set a transactions status to cancelled
   *
   *  @param transaction_id
   *  @return - the data from the api
  */
  public String postTransactionCancelled( String transaction_id) {
    Hashtable p = new Hashtable();
    p.Add("transaction_id",transaction_id);
    return doCurl("POST","/transaction/cancelled",p);
  }


  /**
   * Set a transactions status to complete
   *
   *  @param transaction_id
   *  @param paypal_doexpresscheckoutpayment
   *  @param user_id
   *  @param entity_id
   *  @return - the data from the api
  */
  public String postTransactionComplete( String transaction_id, String paypal_doexpresscheckoutpayment, String user_id, String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("transaction_id",transaction_id);
    p.Add("paypal_doexpresscheckoutpayment",paypal_doexpresscheckoutpayment);
    p.Add("user_id",user_id);
    p.Add("entity_id",entity_id);
    return doCurl("POST","/transaction/complete",p);
  }


  /**
   * Set a transactions status to inprogess
   *
   *  @param transaction_id
   *  @param paypal_setexpresscheckout
   *  @return - the data from the api
  */
  public String postTransactionInprogress( String transaction_id, String paypal_setexpresscheckout) {
    Hashtable p = new Hashtable();
    p.Add("transaction_id",transaction_id);
    p.Add("paypal_setexpresscheckout",paypal_setexpresscheckout);
    return doCurl("POST","/transaction/inprogress",p);
  }


  /**
   * Update user based on email address or social_network/social_network_id
   *
   *  @param email
   *  @param user_id
   *  @param first_name
   *  @param last_name
   *  @param active
   *  @param trust
   *  @param creation_date
   *  @param user_type
   *  @param social_network
   *  @param social_network_id
   *  @param reseller_admin_masheryid
   *  @param group_id
   *  @param admin_upgrader
   *  @param agency_id
   *  @return - the data from the api
  */
  public String postUser( String email, String user_id, String first_name, String last_name, String active, String trust, String creation_date, String user_type, String social_network, String social_network_id, String reseller_admin_masheryid, String group_id, String admin_upgrader, String agency_id) {
    Hashtable p = new Hashtable();
    p.Add("email",email);
    p.Add("user_id",user_id);
    p.Add("first_name",first_name);
    p.Add("last_name",last_name);
    p.Add("active",active);
    p.Add("trust",trust);
    p.Add("creation_date",creation_date);
    p.Add("user_type",user_type);
    p.Add("social_network",social_network);
    p.Add("social_network_id",social_network_id);
    p.Add("reseller_admin_masheryid",reseller_admin_masheryid);
    p.Add("group_id",group_id);
    p.Add("admin_upgrader",admin_upgrader);
    p.Add("agency_id",agency_id);
    return doCurl("POST","/user",p);
  }


  /**
   * With a unique ID address an user can be retrieved
   *
   *  @param user_id
   *  @return - the data from the api
  */
  public String getUser( String user_id) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    return doCurl("GET","/user",p);
  }


  /**
   * With a unique email address an user can be retrieved
   *
   *  @param email
   *  @return - the data from the api
  */
  public String getUserBy_email( String email) {
    Hashtable p = new Hashtable();
    p.Add("email",email);
    return doCurl("GET","/user/by_email",p);
  }


  /**
   * Returns all the users that match the supplied group_id
   *
   *  @param group_id
   *  @return - the data from the api
  */
  public String getUserBy_groupid( String group_id) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    return doCurl("GET","/user/by_groupid",p);
  }


  /**
   * Returns all the users that match the supplied reseller_admin_masheryid
   *
   *  @param reseller_admin_masheryid
   *  @return - the data from the api
  */
  public String getUserBy_reseller_admin_masheryid( String reseller_admin_masheryid) {
    Hashtable p = new Hashtable();
    p.Add("reseller_admin_masheryid",reseller_admin_masheryid);
    return doCurl("GET","/user/by_reseller_admin_masheryid",p);
  }


  /**
   * With a unique ID address an user can be retrieved
   *
   *  @param name
   *  @param id
   *  @return - the data from the api
  */
  public String getUserBy_social_media( String name, String id) {
    Hashtable p = new Hashtable();
    p.Add("name",name);
    p.Add("id",id);
    return doCurl("GET","/user/by_social_media",p);
  }


  /**
   * Removes group_admin privileges from a specified user
   *
   *  @param user_id
   *  @return - the data from the api
  */
  public String postUserGroup_admin_remove( String user_id) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    return doCurl("POST","/user/group_admin_remove",p);
  }


  /**
   * Removes reseller privileges from a specified user
   *
   *  @param user_id
   *  @return - the data from the api
  */
  public String postUserReseller_remove( String user_id) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    return doCurl("POST","/user/reseller_remove",p);
  }


  /**
   * Deletes a specific social network from a user
   *
   *  @param user_id
   *  @param social_network
   *  @return - the data from the api
  */
  public String deleteUserSocial_network( String user_id, String social_network) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    p.Add("social_network",social_network);
    return doCurl("DELETE","/user/social_network",p);
  }


  /**
   * Shows what would be emitted by a view, given a document
   *
   *  @param database - the database being worked on e.g. entities
   *  @param designdoc - the design document containing the view e.g. _design/report
   *  @param view - the name of the view to be tested e.g. bydate
   *  @param doc - the JSON document to be analysed e.g. {}
   *  @return - the data from the api
  */
  public String getViewhelper( String database, String designdoc, String view, String doc) {
    Hashtable p = new Hashtable();
    p.Add("database",database);
    p.Add("designdoc",designdoc);
    p.Add("view",view);
    p.Add("doc",doc);
    return doCurl("GET","/viewhelper",p);
  }


}


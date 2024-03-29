2022/12/14 12:36:37 STARTUP Redis server: tcp://127.0.0.1:6379
2022/12/14 12:36:37 STARTUP ElasticSearch server: http://172.22.12.49:9200
2022/12/14 12:36:37 STARTUP ES view server: http://172.22.114.129:63300/view/view
2022/12/14 12:36:37 STARTUP CouchDB server: http://wolf_staging:******@172.22.10.201:5984
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
   * With a 192 id get remote entity data
   *
   *  @param oneninetwo_id
   *  @return - the data from the api
  */
  public String GET192Get( String oneninetwo_id) {
    Hashtable p = new Hashtable();
    p.Add("oneninetwo_id",oneninetwo_id);
    return doCurl("GET","/192/get",p);
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
  public String GETActivity_stream( String type, String country, String latitude_1, String longitude_1, String latitude_2, String longitude_2, String number_results, String unique_action) {
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
  public String POSTActivity_stream( String entity_id, String entity_name, String type, String country, String longitude, String latitude) {
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
   * Get all entities in which live ads have the matched reseller_masheryid.
   *
   *  @param country
   *  @param reseller_masheryid
   *  @param name_only - If true the query result contains entity name only; otherwise, the entity object.
   *  @param name_match - Filter the result in which the name contains the given text.
   *  @param skip
   *  @param take - Set 0 to get all result. However, if name_only=false, only 100 objects at most will be returned to prevent oversized response body.
   *  @return - the data from the api
  */
  public String GETAdvertiserBy_reseller_masheryid( String country, String reseller_masheryid, String name_only, String name_match, String skip, String take) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    p.Add("reseller_masheryid",reseller_masheryid);
    p.Add("name_only",name_only);
    p.Add("name_match",name_match);
    p.Add("skip",skip);
    p.Add("take",take);
    return doCurl("GET","/advertiser/by_reseller_masheryid",p);
  }


  /**
   * Get all advertisers that have been updated from a give date for a given reseller
   *
   *  @param from_date
   *  @param country
   *  @return - the data from the api
  */
  public String GETAdvertiserUpdated( String from_date, String country) {
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
  public String GETAdvertiserUpdatedBy_publisher( String publisher_id, String from_date, String country) {
    Hashtable p = new Hashtable();
    p.Add("publisher_id",publisher_id);
    p.Add("from_date",from_date);
    p.Add("country",country);
    return doCurl("GET","/advertiser/updated/by_publisher",p);
  }


  /**
   * Check that the advertiser has a premium inventory
   *
   *  @param type
   *  @param category_id - The category of the advertiser
   *  @param location_id - The location of the advertiser
   *  @param publisher_id - The publisher of the advertiser
   *  @return - the data from the api
  */
  public String GETAdvertisersPremiumInventorycheck( String type, String category_id, String location_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("type",type);
    p.Add("category_id",category_id);
    p.Add("location_id",location_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("GET","/advertisers/premium/inventorycheck",p);
  }


  /**
   * Delete an association
   *
   *  @param association_id
   *  @return - the data from the api
  */
  public String DELETEAssociation( String association_id) {
    Hashtable p = new Hashtable();
    p.Add("association_id",association_id);
    return doCurl("DELETE","/association",p);
  }


  /**
   * Fetch an association
   *
   *  @param association_id
   *  @return - the data from the api
  */
  public String GETAssociation( String association_id) {
    Hashtable p = new Hashtable();
    p.Add("association_id",association_id);
    return doCurl("GET","/association",p);
  }


  /**
   * Will create a new association or update an existing one
   *
   *  @param association_id
   *  @param association_name
   *  @param association_url
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTAssociation( String association_id, String association_name, String association_url, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("association_id",association_id);
    p.Add("association_name",association_name);
    p.Add("association_url",association_url);
    p.Add("filedata",filedata);
    return doCurl("POST","/association",p);
  }


  /**
   * The search matches a category name on a given string and language.
   *
   *  @param str - A string to search against, E.g. Plumbers e.g. but
   *  @param language - An ISO compatible language code, E.g. en e.g. en
   *  @param mapped_to_partner - Only return CI categories that have a partner mapping
   *  @return - the data from the api
  */
  public String GETAutocompleteCategory( String str, String language, String mapped_to_partner) {
    Hashtable p = new Hashtable();
    p.Add("str",str);
    p.Add("language",language);
    p.Add("mapped_to_partner",mapped_to_partner);
    return doCurl("GET","/autocomplete/category",p);
  }


  /**
   * The search matches a category name and ID on a given string and language.
   *
   *  @param str - A string to search against, E.g. Plumbers e.g. but
   *  @param language - An ISO compatible language code, E.g. en e.g. en
   *  @param mapped_to_partner - Only return CI categories that have a partner mapping
   *  @return - the data from the api
  */
  public String GETAutocompleteCategoryId( String str, String language, String mapped_to_partner) {
    Hashtable p = new Hashtable();
    p.Add("str",str);
    p.Add("language",language);
    p.Add("mapped_to_partner",mapped_to_partner);
    return doCurl("GET","/autocomplete/category/id",p);
  }


  /**
   * The search matches a category name or synonym on a given string and language.
   *
   *  @param str - A string to search against, E.g. Plumbers e.g. but
   *  @param language - An ISO compatible language code, E.g. en e.g. en
   *  @return - the data from the api
  */
  public String GETAutocompleteKeyword( String str, String language) {
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
  public String GETAutocompleteLocation( String str, String country, String language) {
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
  public String GETAutocompleteLocationBy_resolution( String str, String country, String resolution) {
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
   *  @param status
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
   *  @param allow_no_address
   *  @param allow_no_phone
   *  @param additional_telephone_number
   *  @param email
   *  @param website
   *  @param payment_types - Payment types separated by comma
   *  @param tags - Tags separated by comma
   *  @param category_id
   *  @param category_type
   *  @param featured_message_text - Featured message content
   *  @param featured_message_url - Featured message URL
   *  @param do_not_display
   *  @param orderonline
   *  @param delivers
   *  @param referrer_url
   *  @param referrer_name
   *  @param destructive
   *  @param delete_mode - The type of object contribution deletion
   *  @param master_entity_id - The entity you want this data to go to
   *  @param no_merge_on_error - If true, data duplication error will be returned when a matched entity is found. If false, such error is suppressed and data is merged into the matched entity.
   *  @return - the data from the api
  */
  public String PUTBusiness( String name, String status, String building_number, String branch_name, String address1, String address2, String address3, String district, String town, String county, String province, String postcode, String country, String latitude, String longitude, String timezone, String telephone_number, String allow_no_address, String allow_no_phone, String additional_telephone_number, String email, String website, String payment_types, String tags, String category_id, String category_type, String featured_message_text, String featured_message_url, String do_not_display, String orderonline, String delivers, String referrer_url, String referrer_name, String destructive, String delete_mode, String master_entity_id, String no_merge_on_error) {
    Hashtable p = new Hashtable();
    p.Add("name",name);
    p.Add("status",status);
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
    p.Add("allow_no_address",allow_no_address);
    p.Add("allow_no_phone",allow_no_phone);
    p.Add("additional_telephone_number",additional_telephone_number);
    p.Add("email",email);
    p.Add("website",website);
    p.Add("payment_types",payment_types);
    p.Add("tags",tags);
    p.Add("category_id",category_id);
    p.Add("category_type",category_type);
    p.Add("featured_message_text",featured_message_text);
    p.Add("featured_message_url",featured_message_url);
    p.Add("do_not_display",do_not_display);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("referrer_url",referrer_url);
    p.Add("referrer_name",referrer_name);
    p.Add("destructive",destructive);
    p.Add("delete_mode",delete_mode);
    p.Add("master_entity_id",master_entity_id);
    p.Add("no_merge_on_error",no_merge_on_error);
    return doCurl("PUT","/business",p);
  }


  /**
   * Create entity via JSON
   *
   *  @param json - Business JSON
   *  @param country - The country to fetch results for e.g. gb
   *  @param timezone
   *  @param master_entity_id - The entity you want this data to go to
   *  @param allow_no_address
   *  @param allow_no_phone
   *  @param queue_priority
   *  @param skip_dedup_check - If true, skip checking on existing supplier ID, phone numbers, etc.
   *  @return - the data from the api
  */
  public String PUTBusinessJson( String json, String country, String timezone, String master_entity_id, String allow_no_address, String allow_no_phone, String queue_priority, String skip_dedup_check) {
    Hashtable p = new Hashtable();
    p.Add("json",json);
    p.Add("country",country);
    p.Add("timezone",timezone);
    p.Add("master_entity_id",master_entity_id);
    p.Add("allow_no_address",allow_no_address);
    p.Add("allow_no_phone",allow_no_phone);
    p.Add("queue_priority",queue_priority);
    p.Add("skip_dedup_check",skip_dedup_check);
    return doCurl("PUT","/business/json",p);
  }


  /**
   * Create entity via JSON
   *
   *  @param entity_id - The entity to add rich data too
   *  @param json - The rich data to add to the entity
   *  @return - the data from the api
  */
  public String POSTBusinessJsonProcess( String entity_id, String json) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("json",json);
    return doCurl("POST","/business/json/process",p);
  }


  /**
   * Delete a business tool with a specified tool_id
   *
   *  @param tool_id
   *  @return - the data from the api
  */
  public String DELETEBusiness_tool( String tool_id) {
    Hashtable p = new Hashtable();
    p.Add("tool_id",tool_id);
    return doCurl("DELETE","/business_tool",p);
  }


  /**
   * Returns business tool that matches a given tool id
   *
   *  @param tool_id
   *  @return - the data from the api
  */
  public String GETBusiness_tool( String tool_id) {
    Hashtable p = new Hashtable();
    p.Add("tool_id",tool_id);
    return doCurl("GET","/business_tool",p);
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
  public String POSTBusiness_tool( String tool_id, String country, String headline, String description, String link_url, String active) {
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
   *  @param activeonly
   *  @return - the data from the api
  */
  public String GETBusiness_toolBy_masheryid( String country, String activeonly) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    p.Add("activeonly",activeonly);
    return doCurl("GET","/business_tool/by_masheryid",p);
  }


  /**
   * Assigns a Call To Action to a Business Tool
   *
   *  @param tool_id
   *  @param enablecta
   *  @param cta_id
   *  @param slug
   *  @param nomodal
   *  @param type
   *  @param headline
   *  @param textshort
   *  @param link
   *  @param linklabel
   *  @param textlong
   *  @param textoutro
   *  @param bullets
   *  @param masheryids
   *  @param imgurl
   *  @param custombranding
   *  @param customcol
   *  @param custombkg
   *  @param customctacol
   *  @param customctabkg
   *  @param custominfocol
   *  @param custominfobkg
   *  @return - the data from the api
  */
  public String POSTBusiness_toolCta( String tool_id, String enablecta, String cta_id, String slug, String nomodal, String type, String headline, String textshort, String link, String linklabel, String textlong, String textoutro, String bullets, String masheryids, String imgurl, String custombranding, String customcol, String custombkg, String customctacol, String customctabkg, String custominfocol, String custominfobkg) {
    Hashtable p = new Hashtable();
    p.Add("tool_id",tool_id);
    p.Add("enablecta",enablecta);
    p.Add("cta_id",cta_id);
    p.Add("slug",slug);
    p.Add("nomodal",nomodal);
    p.Add("type",type);
    p.Add("headline",headline);
    p.Add("textshort",textshort);
    p.Add("link",link);
    p.Add("linklabel",linklabel);
    p.Add("textlong",textlong);
    p.Add("textoutro",textoutro);
    p.Add("bullets",bullets);
    p.Add("masheryids",masheryids);
    p.Add("imgurl",imgurl);
    p.Add("custombranding",custombranding);
    p.Add("customcol",customcol);
    p.Add("custombkg",custombkg);
    p.Add("customctacol",customctacol);
    p.Add("customctabkg",customctabkg);
    p.Add("custominfocol",custominfocol);
    p.Add("custominfobkg",custominfobkg);
    return doCurl("POST","/business_tool/cta",p);
  }


  /**
   * Assigns a Business Tool image
   *
   *  @param tool_id
   *  @param assignimage
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTBusiness_toolImage( String tool_id, String assignimage, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("tool_id",tool_id);
    p.Add("assignimage",assignimage);
    p.Add("filedata",filedata);
    return doCurl("POST","/business_tool/image",p);
  }


  /**
   * Assigns a Business Tool image
   *
   *  @param tool_id
   *  @param image_url
   *  @return - the data from the api
  */
  public String POSTBusiness_toolImageBy_url( String tool_id, String image_url) {
    Hashtable p = new Hashtable();
    p.Add("tool_id",tool_id);
    p.Add("image_url",image_url);
    return doCurl("POST","/business_tool/image/by_url",p);
  }


  /**
   * With a known cache key get the data from cache
   *
   *  @param cache_key
   *  @param use_compression
   *  @return - the data from the api
  */
  public String GETCache( String cache_key, String use_compression) {
    Hashtable p = new Hashtable();
    p.Add("cache_key",cache_key);
    p.Add("use_compression",use_compression);
    return doCurl("GET","/cache",p);
  }


  /**
   * Add some data to the cache with a given expiry
   *
   *  @param cache_key
   *  @param expiry - The cache expiry in seconds
   *  @param data - The data to cache
   *  @param use_compression
   *  @return - the data from the api
  */
  public String POSTCache( String cache_key, String expiry, String data, String use_compression) {
    Hashtable p = new Hashtable();
    p.Add("cache_key",cache_key);
    p.Add("expiry",expiry);
    p.Add("data",data);
    p.Add("use_compression",use_compression);
    return doCurl("POST","/cache",p);
  }


  /**
   * Returns the supplied wolf category object by fetching the supplied category_id from our categories object.
   *
   *  @param category_id
   *  @return - the data from the api
  */
  public String GETCategory( String category_id) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    return doCurl("GET","/category",p);
  }


  /**
   * With a known category id, an category object can be added.
   *
   *  @param category_id
   *  @param language
   *  @param name
   *  @return - the data from the api
  */
  public String PUTCategory( String category_id, String language, String name) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("language",language);
    p.Add("name",name);
    return doCurl("PUT","/category",p);
  }


  /**
   * Returns all Central Index categories and associated data
   *
   *  @param partner
   *  @return - the data from the api
  */
  public String GETCategoryAll( String partner) {
    Hashtable p = new Hashtable();
    p.Add("partner",partner);
    return doCurl("GET","/category/all",p);
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
  public String POSTCategoryMappings( String category_id, String type, String id, String name) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("type",type);
    p.Add("id",id);
    p.Add("name",name);
    return doCurl("POST","/category/mappings",p);
  }


  /**
   * With a known category id, a mapping object can be deleted.
   *
   *  @param category_id
   *  @param category_type
   *  @param mapped_id
   *  @return - the data from the api
  */
  public String DELETECategoryMappings( String category_id, String category_type, String mapped_id) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("category_type",category_type);
    p.Add("mapped_id",mapped_id);
    return doCurl("DELETE","/category/mappings",p);
  }


  /**
   * Allows a category object to merged with another
   *
   *  @param from
   *  @param to
   *  @return - the data from the api
  */
  public String POSTCategoryMerge( String from, String to) {
    Hashtable p = new Hashtable();
    p.Add("from",from);
    p.Add("to",to);
    return doCurl("POST","/category/merge",p);
  }


  /**
   * With a known category id, an synonym object can be added.
   *
   *  @param category_id
   *  @param synonym
   *  @param language
   *  @return - the data from the api
  */
  public String POSTCategorySynonym( String category_id, String synonym, String language) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("synonym",synonym);
    p.Add("language",language);
    return doCurl("POST","/category/synonym",p);
  }


  /**
   * With a known category id, a synonyms object can be removed.
   *
   *  @param category_id
   *  @param synonym
   *  @param language
   *  @return - the data from the api
  */
  public String DELETECategorySynonym( String category_id, String synonym, String language) {
    Hashtable p = new Hashtable();
    p.Add("category_id",category_id);
    p.Add("synonym",synonym);
    p.Add("language",language);
    return doCurl("DELETE","/category/synonym",p);
  }


  /**
   * Get the contract from the ID supplied
   *
   *  @param contract_id
   *  @return - the data from the api
  */
  public String GETContract( String contract_id) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    return doCurl("GET","/contract",p);
  }


  /**
   * Get the active contracts from the ID supplied
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String GETContractBy_entity_id( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/contract/by_entity_id",p);
  }


  /**
   * Get a contract from the payment provider id supplied
   *
   *  @param payment_provider
   *  @param payment_provider_id
   *  @return - the data from the api
  */
  public String GETContractBy_payment_provider_id( String payment_provider, String payment_provider_id) {
    Hashtable p = new Hashtable();
    p.Add("payment_provider",payment_provider);
    p.Add("payment_provider_id",payment_provider_id);
    return doCurl("GET","/contract/by_payment_provider_id",p);
  }


  /**
   * Get the active contracts from the ID supplied
   *
   *  @param user_id
   *  @return - the data from the api
  */
  public String GETContractBy_user_id( String user_id) {
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
  public String POSTContractCancel( String contract_id) {
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
   *  @param taxrate
   *  @param billing_period
   *  @param source
   *  @param channel
   *  @param campaign
   *  @param referrer_domain
   *  @param referrer_name
   *  @param flatpack_id
   *  @return - the data from the api
  */
  public String POSTContractCreate( String entity_id, String user_id, String payment_provider, String basket, String taxrate, String billing_period, String source, String channel, String campaign, String referrer_domain, String referrer_name, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("user_id",user_id);
    p.Add("payment_provider",payment_provider);
    p.Add("basket",basket);
    p.Add("taxrate",taxrate);
    p.Add("billing_period",billing_period);
    p.Add("source",source);
    p.Add("channel",channel);
    p.Add("campaign",campaign);
    p.Add("referrer_domain",referrer_domain);
    p.Add("referrer_name",referrer_name);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("POST","/contract/create",p);
  }


  /**
   * Activate a contract that is free
   *
   *  @param contract_id
   *  @param user_name
   *  @param user_surname
   *  @param user_email_address
   *  @return - the data from the api
  */
  public String POSTContractFreeactivate( String contract_id, String user_name, String user_surname, String user_email_address) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    p.Add("user_name",user_name);
    p.Add("user_surname",user_surname);
    p.Add("user_email_address",user_email_address);
    return doCurl("POST","/contract/freeactivate",p);
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
  public String POSTContractPaymentFailure( String contract_id, String failure_reason, String payment_date, String amount, String currency, String response) {
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
  public String POSTContractPaymentSetup( String contract_id, String payment_provider_id, String payment_provider_profile, String user_name, String user_surname, String user_billing_address, String user_email_address) {
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
  public String POSTContractPaymentSuccess( String contract_id, String payment_date, String amount, String currency, String response) {
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
  public String POSTContractProvision( String contract_id) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    return doCurl("POST","/contract/provision",p);
  }


  /**
   * Ensures contract has been cancelled for a given id, expected to be called from stripe on deletion of subscription
   *
   *  @param contract_id
   *  @return - the data from the api
  */
  public String POSTContractSubscriptionended( String contract_id) {
    Hashtable p = new Hashtable();
    p.Add("contract_id",contract_id);
    return doCurl("POST","/contract/subscriptionended",p);
  }


  /**
   * Get the contract log from the ID supplied
   *
   *  @param contract_log_id
   *  @return - the data from the api
  */
  public String GETContract_log( String contract_log_id) {
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
  public String POSTContract_log( String contract_id, String date, String payment_provider, String response, String success, String amount, String currency) {
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
  public String GETContract_logBy_contract_id( String contract_id, String page, String per_page) {
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
  public String GETContract_logBy_payment_provider( String payment_provider, String page, String per_page) {
    Hashtable p = new Hashtable();
    p.Add("payment_provider",payment_provider);
    p.Add("page",page);
    p.Add("per_page",per_page);
    return doCurl("GET","/contract_log/by_payment_provider",p);
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
   *  @param claimProductId
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
  public String POSTCountry( String country_id, String name, String synonyms, String continentName, String continent, String geonameId, String dbpediaURL, String freebaseURL, String population, String currencyCode, String languages, String areaInSqKm, String capital, String east, String west, String north, String south, String claimProductId, String claimMethods, String twilio_sms, String twilio_phone, String twilio_voice, String currency_symbol, String currency_symbol_html, String postcodeLookupActive, String addressFields, String addressMatching, String dateFormat, String iso_3166_alpha_3, String iso_3166_numeric) {
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
    p.Add("claimProductId",claimProductId);
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
  public String GETCountry( String country_id) {
    Hashtable p = new Hashtable();
    p.Add("country_id",country_id);
    return doCurl("GET","/country",p);
  }


  /**
   * An API call to fetch a crash report by its ID
   *
   *  @param crash_report_id - The crash report to pull
   *  @return - the data from the api
  */
  public String GETCrash_report( String crash_report_id) {
    Hashtable p = new Hashtable();
    p.Add("crash_report_id",crash_report_id);
    return doCurl("GET","/crash_report",p);
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
  public String POSTEmail( String to_email_address, String reply_email_address, String source_account, String subject, String body, String html_body) {
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
   * This entity isn't really supported anymore. You probably want PUT /business. Only to be used for testing.
   *
   *  @param type
   *  @param scope
   *  @param country
   *  @param timezone
   *  @param trust
   *  @param our_data
   *  @return - the data from the api
  */
  public String PUTEntity( String type, String scope, String country, String timezone, String trust, String our_data) {
    Hashtable p = new Hashtable();
    p.Add("type",type);
    p.Add("scope",scope);
    p.Add("country",country);
    p.Add("timezone",timezone);
    p.Add("trust",trust);
    p.Add("our_data",our_data);
    return doCurl("PUT","/entity",p);
  }


  /**
   * Allows a whole entity to be pulled from the datastore by its unique id
   *
   *  @param entity_id - The unique entity ID e.g. 379236608286720
   *  @param domain
   *  @param path
   *  @param data_filter
   *  @param filter_by_confidence
   *  @return - the data from the api
  */
  public String GETEntity( String entity_id, String domain, String path, String data_filter, String filter_by_confidence) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("data_filter",data_filter);
    p.Add("filter_by_confidence",filter_by_confidence);
    return doCurl("GET","/entity",p);
  }


  /**
   * With a known entity id, an add can be updated.
   *
   *  @param entity_id
   *  @param add_referrer_url
   *  @param add_referrer_name
   *  @return - the data from the api
  */
  public String POSTEntityAdd( String entity_id, String add_referrer_url, String add_referrer_name) {
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
  public String DELETEEntityAdvertiser( String entity_id, String gen_id) {
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
  public String POSTEntityAdvertiserCancel( String entity_id, String publisher_id, String reseller_ref, String reseller_agent_id) {
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
   *  @param loc_tags
   *  @param region_tags
   *  @param max_tags
   *  @param max_locations
   *  @param expiry_date
   *  @param is_national
   *  @param is_regional
   *  @param language
   *  @param reseller_ref
   *  @param reseller_agent_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String POSTEntityAdvertiserCreate( String entity_id, String tags, String locations, String loc_tags, String region_tags, String max_tags, String max_locations, String expiry_date, String is_national, String is_regional, String language, String reseller_ref, String reseller_agent_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("tags",tags);
    p.Add("locations",locations);
    p.Add("loc_tags",loc_tags);
    p.Add("region_tags",region_tags);
    p.Add("max_tags",max_tags);
    p.Add("max_locations",max_locations);
    p.Add("expiry_date",expiry_date);
    p.Add("is_national",is_national);
    p.Add("is_regional",is_regional);
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
  public String POSTEntityAdvertiserLocation( String entity_id, String gen_id, String locations_to_add, String locations_to_remove) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    p.Add("locations_to_add",locations_to_add);
    p.Add("locations_to_remove",locations_to_remove);
    return doCurl("POST","/entity/advertiser/location",p);
  }


  /**
   * With a known entity id, a premium advertiser is cancelled
   *
   *  @param entity_id
   *  @param publisher_id
   *  @param type
   *  @param category_id - The category of the advertiser
   *  @param location_id - The location of the advertiser
   *  @return - the data from the api
  */
  public String POSTEntityAdvertiserPremiumCancel( String entity_id, String publisher_id, String type, String category_id, String location_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("publisher_id",publisher_id);
    p.Add("type",type);
    p.Add("category_id",category_id);
    p.Add("location_id",location_id);
    return doCurl("POST","/entity/advertiser/premium/cancel",p);
  }


  /**
   * With a known entity id, a premium advertiser is added
   *
   *  @param entity_id
   *  @param type
   *  @param category_id - The category of the advertiser
   *  @param location_id - The location of the advertiser
   *  @param expiry_date
   *  @param reseller_ref
   *  @param reseller_agent_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String POSTEntityAdvertiserPremiumCreate( String entity_id, String type, String category_id, String location_id, String expiry_date, String reseller_ref, String reseller_agent_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("type",type);
    p.Add("category_id",category_id);
    p.Add("location_id",location_id);
    p.Add("expiry_date",expiry_date);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_agent_id",reseller_agent_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("POST","/entity/advertiser/premium/create",p);
  }


  /**
   * Renews an existing premium advertiser in an entity
   *
   *  @param entity_id
   *  @param type
   *  @param category_id - The category of the advertiser
   *  @param location_id - The location of the advertiser
   *  @param expiry_date
   *  @param reseller_ref
   *  @param reseller_agent_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String POSTEntityAdvertiserPremiumRenew( String entity_id, String type, String category_id, String location_id, String expiry_date, String reseller_ref, String reseller_agent_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("type",type);
    p.Add("category_id",category_id);
    p.Add("location_id",location_id);
    p.Add("expiry_date",expiry_date);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_agent_id",reseller_agent_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("POST","/entity/advertiser/premium/renew",p);
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
  public String POSTEntityAdvertiserRenew( String entity_id, String expiry_date, String publisher_id, String reseller_ref, String reseller_agent_id) {
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
  public String POSTEntityAdvertiserTag( String gen_id, String entity_id, String language, String tags_to_add, String tags_to_remove) {
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
   *  @param loc_tags
   *  @param is_regional
   *  @param region_tags
   *  @param extra_tags
   *  @param extra_locations
   *  @param is_national
   *  @param language
   *  @param reseller_ref
   *  @param reseller_agent_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String POSTEntityAdvertiserUpsell( String entity_id, String tags, String locations, String loc_tags, String is_regional, String region_tags, String extra_tags, String extra_locations, String is_national, String language, String reseller_ref, String reseller_agent_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("tags",tags);
    p.Add("locations",locations);
    p.Add("loc_tags",loc_tags);
    p.Add("is_regional",is_regional);
    p.Add("region_tags",region_tags);
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
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param is_national
   *  @param limit - The number of advertisers that are to be returned
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @param benchmark
   *  @return - the data from the api
  */
  public String GETEntityAdvertisers( String tag, String where, String orderonline, String delivers, String isClaimed, String is_national, String limit, String country, String language, String benchmark) {
    Hashtable p = new Hashtable();
    p.Add("tag",tag);
    p.Add("where",where);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("is_national",is_national);
    p.Add("limit",limit);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("benchmark",benchmark);
    return doCurl("GET","/entity/advertisers",p);
  }


  /**
   * Search for matching entities in a specified location that are advertisers and return a random selection upto the limit requested
   *
   *  @param location - The location to get results for. E.g. Dublin
   *  @param is_national
   *  @param limit - The number of advertisers that are to be returned
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @return - the data from the api
  */
  public String GETEntityAdvertisersBy_location( String location, String is_national, String limit, String country, String language) {
    Hashtable p = new Hashtable();
    p.Add("location",location);
    p.Add("is_national",is_national);
    p.Add("limit",limit);
    p.Add("country",country);
    p.Add("language",language);
    return doCurl("GET","/entity/advertisers/by_location",p);
  }


  /**
   * Check if an entity has an advert from a specified publisher
   *
   *  @param entity_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String GETEntityAdvertisersInventorycheck( String entity_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("GET","/entity/advertisers/inventorycheck",p);
  }


  /**
   * Get advertisers premium
   *
   *  @param what
   *  @param where
   *  @param type
   *  @param country
   *  @param language
   *  @return - the data from the api
  */
  public String GETEntityAdvertisersPremium( String what, String where, String type, String country, String language) {
    Hashtable p = new Hashtable();
    p.Add("what",what);
    p.Add("where",where);
    p.Add("type",type);
    p.Add("country",country);
    p.Add("language",language);
    return doCurl("GET","/entity/advertisers/premium",p);
  }


  /**
   * Deleteing an affiliate adblock from a known entity
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityAffiliate_adblock( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/affiliate_adblock",p);
  }


  /**
   * Adding an affiliate adblock to a known entity
   *
   *  @param entity_id
   *  @param adblock - Number of results returned per page
   *  @return - the data from the api
  */
  public String POSTEntityAffiliate_adblock( String entity_id, String adblock) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("adblock",adblock);
    return doCurl("POST","/entity/affiliate_adblock",p);
  }


  /**
   * With a known entity id, an affiliate link object can be added.
   *
   *  @param entity_id
   *  @param affiliate_name
   *  @param affiliate_link
   *  @param affiliate_message
   *  @param affiliate_logo
   *  @param affiliate_action
   *  @return - the data from the api
  */
  public String POSTEntityAffiliate_link( String entity_id, String affiliate_name, String affiliate_link, String affiliate_message, String affiliate_logo, String affiliate_action) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("affiliate_name",affiliate_name);
    p.Add("affiliate_link",affiliate_link);
    p.Add("affiliate_message",affiliate_message);
    p.Add("affiliate_logo",affiliate_logo);
    p.Add("affiliate_action",affiliate_action);
    return doCurl("POST","/entity/affiliate_link",p);
  }


  /**
   * Allows an affiliate link object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityAffiliate_link( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/affiliate_link",p);
  }


  /**
   * Add/edit an annoucement object to an existing entity.
   *
   *  @param entity_id
   *  @param announcement_id
   *  @param headline
   *  @param body
   *  @param link_label
   *  @param link
   *  @param terms_link
   *  @param publish_date
   *  @param expiry_date
   *  @param media_type
   *  @param image_url
   *  @param video_url
   *  @param type - Type of announcement, which affects how it is displayed.
   *  @return - the data from the api
  */
  public String POSTEntityAnnouncement( String entity_id, String announcement_id, String headline, String body, String link_label, String link, String terms_link, String publish_date, String expiry_date, String media_type, String image_url, String video_url, String type) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("announcement_id",announcement_id);
    p.Add("headline",headline);
    p.Add("body",body);
    p.Add("link_label",link_label);
    p.Add("link",link);
    p.Add("terms_link",terms_link);
    p.Add("publish_date",publish_date);
    p.Add("expiry_date",expiry_date);
    p.Add("media_type",media_type);
    p.Add("image_url",image_url);
    p.Add("video_url",video_url);
    p.Add("type",type);
    return doCurl("POST","/entity/announcement",p);
  }


  /**
   * Fetch an announcement object from an existing entity.
   *
   *  @param entity_id
   *  @param announcement_id
   *  @return - the data from the api
  */
  public String GETEntityAnnouncement( String entity_id, String announcement_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("announcement_id",announcement_id);
    return doCurl("GET","/entity/announcement",p);
  }


  /**
   * Remove an announcement object to an existing entity.
   *
   *  @param entity_id
   *  @param announcement_id
   *  @return - the data from the api
  */
  public String DELETEEntityAnnouncement( String entity_id, String announcement_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("announcement_id",announcement_id);
    return doCurl("DELETE","/entity/announcement",p);
  }


  /**
   * Will create a new association_membership or update an existing one
   *
   *  @param entity_id
   *  @param association_id
   *  @param association_member_url
   *  @param association_member_id
   *  @return - the data from the api
  */
  public String POSTEntityAssociation_membership( String entity_id, String association_id, String association_member_url, String association_member_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("association_id",association_id);
    p.Add("association_member_url",association_member_url);
    p.Add("association_member_id",association_member_id);
    return doCurl("POST","/entity/association_membership",p);
  }


  /**
   * Allows a association_membership object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityAssociation_membership( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/association_membership",p);
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
  public String POSTEntityBackground( String entity_id, String number_of_employees, String turnover, String net_profit, String vat_number, String duns_number, String registered_company_number) {
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
   * With a known entity id, a brand object can be added.
   *
   *  @param entity_id
   *  @param value
   *  @return - the data from the api
  */
  public String POSTEntityBrand( String entity_id, String value) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("value",value);
    return doCurl("POST","/entity/brand",p);
  }


  /**
   * With a known entity id, a brand object can be deleted.
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityBrand( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/brand",p);
  }


  /**
   * Uploads a CSV file of known format and bulk inserts into DB
   *
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTEntityBulkCsv( String filedata) {
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
  public String GETEntityBulkCsvStatus( String upload_id) {
    Hashtable p = new Hashtable();
    p.Add("upload_id",upload_id);
    return doCurl("GET","/entity/bulk/csv/status",p);
  }


  /**
   * Uploads a JSON file of known format and bulk inserts into DB
   *
   *  @param data
   *  @param new_entities
   *  @return - the data from the api
  */
  public String POSTEntityBulkJson( String data, String new_entities) {
    Hashtable p = new Hashtable();
    p.Add("data",data);
    p.Add("new_entities",new_entities);
    return doCurl("POST","/entity/bulk/json",p);
  }


  /**
   * Shows the current status of a bulk JSON upload
   *
   *  @param upload_id
   *  @return - the data from the api
  */
  public String GETEntityBulkJsonStatus( String upload_id) {
    Hashtable p = new Hashtable();
    p.Add("upload_id",upload_id);
    return doCurl("GET","/entity/bulk/json/status",p);
  }


  /**
   * Fetches the document that matches the given data_source_type and external_id.
   *
   *  @param data_source_type - The data source type of the entity
   *  @param external_id - The external ID of the entity
   *  @return - the data from the api
  */
  public String GETEntityBy_external_id( String data_source_type, String external_id) {
    Hashtable p = new Hashtable();
    p.Add("data_source_type",data_source_type);
    p.Add("external_id",external_id);
    return doCurl("GET","/entity/by_external_id",p);
  }


  /**
   * Get all entities within a specified group
   *
   *  @param group_id
   *  @return - the data from the api
  */
  public String GETEntityBy_groupid( String group_id) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    return doCurl("GET","/entity/by_groupid",p);
  }


  /**
   * Fetches the document that matches the given legacy_url
   *
   *  @param legacy_url - The URL of the entity in the directory it was imported from.
   *  @return - the data from the api
  */
  public String GETEntityBy_legacy_url( String legacy_url) {
    Hashtable p = new Hashtable();
    p.Add("legacy_url",legacy_url);
    return doCurl("GET","/entity/by_legacy_url",p);
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
  public String DELETEEntityBy_supplier( String entity_id, String supplier_masheryid, String supplier_id, String supplier_user_id) {
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
   *  @param supplier_id - The Supplier ID, or a list of supplier IDs separated by comma
   *  @return - the data from the api
  */
  public String GETEntityBy_supplier_id( String supplier_id) {
    Hashtable p = new Hashtable();
    p.Add("supplier_id",supplier_id);
    return doCurl("GET","/entity/by_supplier_id",p);
  }


  /**
   * Get all entities added or claimed by a specific user
   *
   *  @param user_id - The unique user ID of the user with claimed entities e.g. 379236608286720
   *  @param filter
   *  @param skip
   *  @param limit
   *  @return - the data from the api
  */
  public String GETEntityBy_user_id( String user_id, String filter, String skip, String limit) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    p.Add("filter",filter);
    p.Add("skip",skip);
    p.Add("limit",limit);
    return doCurl("GET","/entity/by_user_id",p);
  }


  /**
   * With a known entity id, an category object can be added.
   *
   *  @param entity_id
   *  @param category_id
   *  @param category_type
   *  @return - the data from the api
  */
  public String POSTEntityCategory( String entity_id, String category_id, String category_type) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("category_id",category_id);
    p.Add("category_type",category_type);
    return doCurl("POST","/entity/category",p);
  }


  /**
   * Allows a category object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityCategory( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/category",p);
  }


  /**
   * Fetches the changelog documents that match the given entity_id
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String GETEntityChangelog( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/entity/changelog",p);
  }


  /**
   * Unlike cancel, this operation remove the claim data from the entity
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String DELETEEntityClaim( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/entity/claim",p);
  }


  /**
   * Allow an entity to be claimed by a valid user
   *
   *  @param entity_id
   *  @param claimed_user_id
   *  @param claimed_reseller_id
   *  @param expiry_date
   *  @param claimed_date
   *  @param verified_status - If set to a value, this field will promote the claim to pro mode (expiry aligned with claim expiry)
   *  @param claim_method
   *  @param phone_number
   *  @param referrer_url
   *  @param referrer_name
   *  @param reseller_ref
   *  @param reseller_description
   *  @return - the data from the api
  */
  public String POSTEntityClaim( String entity_id, String claimed_user_id, String claimed_reseller_id, String expiry_date, String claimed_date, String verified_status, String claim_method, String phone_number, String referrer_url, String referrer_name, String reseller_ref, String reseller_description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("claimed_user_id",claimed_user_id);
    p.Add("claimed_reseller_id",claimed_reseller_id);
    p.Add("expiry_date",expiry_date);
    p.Add("claimed_date",claimed_date);
    p.Add("verified_status",verified_status);
    p.Add("claim_method",claim_method);
    p.Add("phone_number",phone_number);
    p.Add("referrer_url",referrer_url);
    p.Add("referrer_name",referrer_name);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_description",reseller_description);
    return doCurl("POST","/entity/claim",p);
  }


  /**
   * Cancel a claim that is on the entity
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String POSTEntityClaimCancel( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("POST","/entity/claim/cancel",p);
  }


  /**
   * Allow an entity to be claimed by a valid user
   *
   *  @param entity_id
   *  @param claimed_user_id
   *  @param reseller_ref
   *  @param reseller_description
   *  @param expiry_date
   *  @param renew_verify - Update the verified_status (where present) as well. Paid claims should do this -- free claims generally will not.
   *  @return - the data from the api
  */
  public String POSTEntityClaimRenew( String entity_id, String claimed_user_id, String reseller_ref, String reseller_description, String expiry_date, String renew_verify) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("claimed_user_id",claimed_user_id);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_description",reseller_description);
    p.Add("expiry_date",expiry_date);
    p.Add("renew_verify",renew_verify);
    return doCurl("POST","/entity/claim/renew",p);
  }


  /**
   * Allow an entity to be claimed by a valid reseller
   *
   *  @param entity_id
   *  @param reseller_ref
   *  @param reseller_description
   *  @return - the data from the api
  */
  public String POSTEntityClaimReseller( String entity_id, String reseller_ref, String reseller_description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("reseller_ref",reseller_ref);
    p.Add("reseller_description",reseller_description);
    return doCurl("POST","/entity/claim/reseller",p);
  }


  /**
   * If an entity is currently claimed then set or remove the verified_entity block (Expiry will match the claim expiry)
   *
   *  @param entity_id
   *  @param verified_status - If set to a value, this field will promote the claim to pro mode. If blank, verified status will be wiped
   *  @return - the data from the api
  */
  public String POSTEntityClaimVerfied_status( String entity_id, String verified_status) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("verified_status",verified_status);
    return doCurl("POST","/entity/claim/verfied_status",p);
  }


  /**
   * Add/change delivers flag for an existing entity - to indicate whether business offers delivery
   *
   *  @param entity_id
   *  @param delivers
   *  @return - the data from the api
  */
  public String POSTEntityDelivers( String entity_id, String delivers) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("delivers",delivers);
    return doCurl("POST","/entity/delivers",p);
  }


  /**
   * Allows a description object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityDescription( String entity_id, String gen_id) {
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
   *  @param gen_id
   *  @return - the data from the api
  */
  public String POSTEntityDescription( String entity_id, String headline, String body, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("headline",headline);
    p.Add("body",body);
    p.Add("gen_id",gen_id);
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
  public String POSTEntityDocument( String entity_id, String name, String filedata) {
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
  public String DELETEEntityDocument( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/document",p);
  }


  /**
   * Upload a document to an entity
   *
   *  @param entity_id
   *  @param document
   *  @return - the data from the api
  */
  public String POSTEntityDocumentBy_url( String entity_id, String document) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("document",document);
    return doCurl("POST","/entity/document/by_url",p);
  }


  /**
   * Allows a email object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityEmail( String entity_id, String gen_id) {
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
  public String POSTEntityEmail( String entity_id, String email_address, String email_description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("email_address",email_address);
    p.Add("email_description",email_description);
    return doCurl("POST","/entity/email",p);
  }


  /**
   * Fetch an emergency statement object from an existing entity.
   *
   *  @param entity_id
   *  @param emergencystatement_id
   *  @return - the data from the api
  */
  public String GETEntityEmergencystatement( String entity_id, String emergencystatement_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("emergencystatement_id",emergencystatement_id);
    return doCurl("GET","/entity/emergencystatement",p);
  }


  /**
   * Add or update an emergency statement object to an existing entity.
   *
   *  @param entity_id
   *  @param id
   *  @param headline
   *  @param body
   *  @param link_label
   *  @param link
   *  @param publish_date
   *  @return - the data from the api
  */
  public String POSTEntityEmergencystatement( String entity_id, String id, String headline, String body, String link_label, String link, String publish_date) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("id",id);
    p.Add("headline",headline);
    p.Add("body",body);
    p.Add("link_label",link_label);
    p.Add("link",link);
    p.Add("publish_date",publish_date);
    return doCurl("POST","/entity/emergencystatement",p);
  }


  /**
   * Remove an emergencystatement object to an existing entity.
   *
   *  @param entity_id
   *  @param emergencystatement_id
   *  @return - the data from the api
  */
  public String DELETEEntityEmergencystatement( String entity_id, String emergencystatement_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("emergencystatement_id",emergencystatement_id);
    return doCurl("DELETE","/entity/emergencystatement",p);
  }


  /**
   * Fetch emergency statement objects from an existing entity.
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String GETEntityEmergencystatements( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/entity/emergencystatements",p);
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
  public String POSTEntityEmployee( String entity_id, String title, String forename, String surname, String job_title, String description, String email, String phone_number) {
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
   * Allows an employee object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityEmployee( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/employee",p);
  }


  /**
   * With a known entity id, an FAQ question and answer can be added.
   *
   *  @param entity_id
   *  @param question
   *  @param answer
   *  @param gen_id
   *  @return - the data from the api
  */
  public String POSTEntityFaq( String entity_id, String question, String answer, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("question",question);
    p.Add("answer",answer);
    p.Add("gen_id",gen_id);
    return doCurl("POST","/entity/faq",p);
  }


  /**
   * With a known entity id, an FAQ question and answer can be removed.
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityFaq( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/faq",p);
  }


  /**
   * Allows a fax object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityFax( String entity_id, String gen_id) {
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
  public String POSTEntityFax( String entity_id, String number, String description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("number",number);
    p.Add("description",description);
    return doCurl("POST","/entity/fax",p);
  }


  /**
   * With a known entity id, a featured message can be added
   *
   *  @param entity_id
   *  @param featured_text
   *  @param featured_url
   *  @return - the data from the api
  */
  public String POSTEntityFeatured_message( String entity_id, String featured_text, String featured_url) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("featured_text",featured_text);
    p.Add("featured_url",featured_url);
    return doCurl("POST","/entity/featured_message",p);
  }


  /**
   * Allows a featured message object to be removed
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String DELETEEntityFeatured_message( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/entity/featured_message",p);
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
  public String POSTEntityGeopoint( String entity_id, String longitude, String latitude, String accuracy) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("longitude",longitude);
    p.Add("latitude",latitude);
    p.Add("accuracy",accuracy);
    return doCurl("POST","/entity/geopoint",p);
  }


  /**
   * With a known entity id, a group  can be added to group members.
   *
   *  @param entity_id
   *  @param group_id
   *  @return - the data from the api
  */
  public String POSTEntityGroup( String entity_id, String group_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("group_id",group_id);
    return doCurl("POST","/entity/group",p);
  }


  /**
   * Allows a group object to be removed from an entities group members
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityGroup( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/group",p);
  }


  /**
   * With a known entity id, a image object can be added.
   *
   *  @param entity_id
   *  @param filedata
   *  @param image_name
   *  @return - the data from the api
  */
  public String POSTEntityImage( String entity_id, String filedata, String image_name) {
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
  public String DELETEEntityImage( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/image",p);
  }


  /**
   * With a known entity id, a image can be retrieved from a url and added.
   *
   *  @param entity_id
   *  @param image_url
   *  @param image_name
   *  @return - the data from the api
  */
  public String POSTEntityImageBy_url( String entity_id, String image_url, String image_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("image_url",image_url);
    p.Add("image_name",image_name);
    return doCurl("POST","/entity/image/by_url",p);
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
  public String POSTEntityInvoice_address( String entity_id, String building_number, String address1, String address2, String address3, String district, String town, String county, String province, String postcode, String address_type) {
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
  public String DELETEEntityInvoice_address( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/entity/invoice_address",p);
  }


  /**
   * With a known entity id, a language object can be deleted.
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityLanguage( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/language",p);
  }


  /**
   * With a known entity id, a language object can be added.
   *
   *  @param entity_id
   *  @param value
   *  @return - the data from the api
  */
  public String POSTEntityLanguage( String entity_id, String value) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("value",value);
    return doCurl("POST","/entity/language",p);
  }


  /**
   * Allows a list description object to be reduced in confidence
   *
   *  @param gen_id
   *  @param entity_id
   *  @return - the data from the api
  */
  public String DELETEEntityList( String gen_id, String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("gen_id",gen_id);
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/entity/list",p);
  }


  /**
   * With a known entity id, a list description object can be added.
   *
   *  @param entity_id
   *  @param headline
   *  @param body
   *  @return - the data from the api
  */
  public String POSTEntityList( String entity_id, String headline, String body) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("headline",headline);
    p.Add("body",body);
    return doCurl("POST","/entity/list",p);
  }


  /**
   * Find all entities in a group
   *
   *  @param group_id - A valid group_id
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @return - the data from the api
  */
  public String GETEntityList_by_group_id( String group_id, String per_page, String page) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    p.Add("per_page",per_page);
    p.Add("page",page);
    return doCurl("GET","/entity/list_by_group_id",p);
  }


  /**
   * Adds/removes loc_tags
   *
   *  @param entity_id
   *  @param gen_id
   *  @param loc_tags_to_add
   *  @param loc_tags_to_remove
   *  @return - the data from the api
  */
  public String POSTEntityLoc_tag( String entity_id, String gen_id, String loc_tags_to_add, String loc_tags_to_remove) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    p.Add("loc_tags_to_add",loc_tags_to_add);
    p.Add("loc_tags_to_remove",loc_tags_to_remove);
    return doCurl("POST","/entity/loc_tag",p);
  }


  /**
   * Allows a phone object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityLogo( String entity_id, String gen_id) {
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
  public String POSTEntityLogo( String entity_id, String filedata, String logo_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("filedata",filedata);
    p.Add("logo_name",logo_name);
    return doCurl("POST","/entity/logo",p);
  }


  /**
   * With a known entity id, a logo can be retrieved from a url and added.
   *
   *  @param entity_id
   *  @param logo_url
   *  @param logo_name
   *  @return - the data from the api
  */
  public String POSTEntityLogoBy_url( String entity_id, String logo_url, String logo_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("logo_url",logo_url);
    p.Add("logo_name",logo_name);
    return doCurl("POST","/entity/logo/by_url",p);
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
   *  @param delete_mode - The type of object contribution deletion
   *  @return - the data from the api
  */
  public String POSTEntityMerge( String from, String to, String override_trust, String uncontribute_masheryid, String uncontribute_userid, String uncontribute_supplierid, String delete_mode) {
    Hashtable p = new Hashtable();
    p.Add("from",from);
    p.Add("to",to);
    p.Add("override_trust",override_trust);
    p.Add("uncontribute_masheryid",uncontribute_masheryid);
    p.Add("uncontribute_userid",uncontribute_userid);
    p.Add("uncontribute_supplierid",uncontribute_supplierid);
    p.Add("delete_mode",delete_mode);
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
  public String POSTEntityMigrate_category( String from, String to, String limit) {
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
  public String POSTEntityName( String entity_id, String name, String formal_name, String branch_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("name",name);
    p.Add("formal_name",formal_name);
    p.Add("branch_name",branch_name);
    return doCurl("POST","/entity/name",p);
  }


  /**
   * With a known entity id, a opening times object can be added. Each day can be either 'closed' to indicate that the entity is closed that day, '24hour' to indicate that the entity is open all day or single/split time ranges can be supplied in 4-digit 24-hour format, such as '09001730' or '09001200,13001700' to indicate hours of opening.
   *
   *  @param entity_id - The id of the entity to edit
   *  @param statement - Statement describing reasons for special opening/closing times
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
  public String POSTEntityOpening_times( String entity_id, String statement, String monday, String tuesday, String wednesday, String thursday, String friday, String saturday, String sunday, String closed, String closed_public_holidays) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("statement",statement);
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
   * With a known entity id, a opening times object can be removed.
   *
   *  @param entity_id - The id of the entity to edit
   *  @return - the data from the api
  */
  public String DELETEEntityOpening_times( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/entity/opening_times",p);
  }


  /**
   * Add an order online to an existing entity - to indicate e-commerce capability.
   *
   *  @param entity_id
   *  @param orderonline
   *  @return - the data from the api
  */
  public String POSTEntityOrderonline( String entity_id, String orderonline) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("orderonline",orderonline);
    return doCurl("POST","/entity/orderonline",p);
  }


  /**
   * Allows a payment_type object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityPayment_type( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/payment_type",p);
  }


  /**
   * With a known entity id, a payment_type object can be added.
   *
   *  @param entity_id - the id of the entity to add the payment type to
   *  @param payment_type - the payment type to add to the entity
   *  @return - the data from the api
  */
  public String POSTEntityPayment_type( String entity_id, String payment_type) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("payment_type",payment_type);
    return doCurl("POST","/entity/payment_type",p);
  }


  /**
   * Allows a new phone object to be added to a specified entity. A new object id will be calculated and returned to you if successful.
   *
   *  @param entity_id
   *  @param number
   *  @param description
   *  @param trackable
   *  @return - the data from the api
  */
  public String POSTEntityPhone( String entity_id, String number, String description, String trackable) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("number",number);
    p.Add("description",description);
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
  public String DELETEEntityPhone( String entity_id, String gen_id) {
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
  public String POSTEntityPostal_address( String entity_id, String building_number, String address1, String address2, String address3, String district, String town, String county, String province, String postcode, String address_type, String do_not_display) {
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
  public String GETEntityProvisionalBy_supplier_id( String supplier_id) {
    Hashtable p = new Hashtable();
    p.Add("supplier_id",supplier_id);
    return doCurl("GET","/entity/provisional/by_supplier_id",p);
  }


  /**
   * removes a given entities supplier/masheryid/user_id content and makes the entity inactive if the entity is un-usable
   *
   *  @param entity_id - The entity to pull
   *  @param purge_masheryid - The purge masheryid to match
   *  @param purge_supplier_id - The purge supplier id to match
   *  @param purge_user_id - The purge user id to match
   *  @param exclude - List of entity fields that are excluded from the purge
   *  @param destructive
   *  @return - the data from the api
  */
  public String POSTEntityPurge( String entity_id, String purge_masheryid, String purge_supplier_id, String purge_user_id, String exclude, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("purge_masheryid",purge_masheryid);
    p.Add("purge_supplier_id",purge_supplier_id);
    p.Add("purge_user_id",purge_user_id);
    p.Add("exclude",exclude);
    p.Add("destructive",destructive);
    return doCurl("POST","/entity/purge",p);
  }


  /**
   * removes a portion of a given entity and makes the entity inactive if the resulting leftover entity is un-usable
   *
   *  @param entity_id - The entity to pull
   *  @param object
   *  @param gen_id - The gen_id of any multi-object being purged
   *  @param purge_masheryid - The purge masheryid to match
   *  @param purge_supplier_id - The purge supplier id to match
   *  @param purge_user_id - The purge user id to match
   *  @param destructive
   *  @return - the data from the api
  */
  public String POSTEntityPurgeBy_object( String entity_id, String _object, String gen_id, String purge_masheryid, String purge_supplier_id, String purge_user_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("object",_object);
    p.Add("gen_id",gen_id);
    p.Add("purge_masheryid",purge_masheryid);
    p.Add("purge_supplier_id",purge_supplier_id);
    p.Add("purge_user_id",purge_user_id);
    p.Add("destructive",destructive);
    return doCurl("POST","/entity/purge/by_object",p);
  }


  /**
   * Deletes a specific review for an entity via Review API
   *
   *  @param entity_id - The entity with the review
   *  @param review_id - The review id
   *  @return - the data from the api
  */
  public String DELETEEntityReview( String entity_id, String review_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("review_id",review_id);
    return doCurl("DELETE","/entity/review",p);
  }


  /**
   * Gets a specific review  for an entity
   *
   *  @param entity_id - The entity with the review
   *  @param review_id - The review id
   *  @return - the data from the api
  */
  public String GETEntityReview( String entity_id, String review_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("review_id",review_id);
    return doCurl("GET","/entity/review",p);
  }


  /**
   * Appends a review to an entity
   *
   *  @param entity_id - the entity to append the review to
   *  @param reviewer_user_id - The user id
   *  @param review_id - The review id. If this is supplied will attempt to update an existing review
   *  @param title - The title of the review
   *  @param content - The full text content of the review
   *  @param star_rating - The rating of the review
   *  @param domain - The domain the review originates from
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTEntityReview( String entity_id, String reviewer_user_id, String review_id, String title, String content, String star_rating, String domain, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("reviewer_user_id",reviewer_user_id);
    p.Add("review_id",review_id);
    p.Add("title",title);
    p.Add("content",content);
    p.Add("star_rating",star_rating);
    p.Add("domain",domain);
    p.Add("filedata",filedata);
    return doCurl("POST","/entity/review",p);
  }


  /**
   * Gets all reviews for an entity
   *
   *  @param entity_id - The entity with the review
   *  @param limit - Limit the number of results returned
   *  @param skip - Number of results skipped
   *  @return - the data from the api
  */
  public String GETEntityReviewList( String entity_id, String limit, String skip) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("limit",limit);
    p.Add("skip",skip);
    return doCurl("GET","/entity/review/list",p);
  }


  /**
   * Allows a list of available revisions to be returned by its entity id
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String GETEntityRevisions( String entity_id) {
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
  public String GETEntityRevisionsByRevisionID( String entity_id, String revision_id) {
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
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param per_page
   *  @param page
   *  @param country
   *  @param language
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @return - the data from the api
  */
  public String GETEntitySearchByboundingbox( String latitude_1, String longitude_1, String latitude_2, String longitude_2, String orderonline, String delivers, String isClaimed, String per_page, String page, String country, String language, String domain, String path, String restrict_category_ids) {
    Hashtable p = new Hashtable();
    p.Add("latitude_1",latitude_1);
    p.Add("longitude_1",longitude_1);
    p.Add("latitude_2",latitude_2);
    p.Add("longitude_2",longitude_2);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    return doCurl("GET","/entity/search/byboundingbox",p);
  }


  /**
   * Search for matching entities
   *
   *  @param where - Location to search for results. E.g. Dublin e.g. Dublin
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param per_page - How many results per page
   *  @param page - What page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @param latitude - The decimal latitude of the search context (optional)
   *  @param longitude - The decimal longitude of the search context (optional)
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @param benchmark
   *  @return - the data from the api
  */
  public String GETEntitySearchBylocation( String where, String orderonline, String delivers, String isClaimed, String per_page, String page, String country, String language, String latitude, String longitude, String domain, String path, String restrict_category_ids, String benchmark) {
    Hashtable p = new Hashtable();
    p.Add("where",where);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    p.Add("benchmark",benchmark);
    return doCurl("GET","/entity/search/bylocation",p);
  }


  /**
   * Search for entities matching the supplied group_id, ordered by nearness
   *
   *  @param group_id - the group_id to search for
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param country - The country to fetch results for e.g. gb
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @param language - An ISO compatible language code, E.g. en
   *  @param latitude - The decimal latitude of the centre point of the search
   *  @param longitude - The decimal longitude of the centre point of the search
   *  @param where - The location to search for
   *  @param domain
   *  @param path
   *  @param unitOfDistance
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @return - the data from the api
  */
  public String GETEntitySearchGroupBynearest( String group_id, String orderonline, String delivers, String isClaimed, String country, String per_page, String page, String language, String latitude, String longitude, String where, String domain, String path, String unitOfDistance, String restrict_category_ids) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("country",country);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("language",language);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("where",where);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("unitOfDistance",unitOfDistance);
    p.Add("restrict_category_ids",restrict_category_ids);
    return doCurl("GET","/entity/search/group/bynearest",p);
  }


  /**
   * Search for entities matching the supplied 'who', ordered by nearness. NOTE if you want to see any advertisers then append MASHERYID (even if using API key) and include_ads=true to get your ads matching that keyword and the derived location.
   *
   *  @param keyword - What to get results for. E.g. cafe e.g. cafe
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param country - The country to fetch results for e.g. gb
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @param language - An ISO compatible language code, E.g. en
   *  @param latitude - The decimal latitude of the centre point of the search
   *  @param longitude - The decimal longitude of the centre point of the search
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @param include_ads - Find nearby advertisers with tags that match the keyword
   *  @return - the data from the api
  */
  public String GETEntitySearchKeywordBynearest( String keyword, String orderonline, String delivers, String isClaimed, String country, String per_page, String page, String language, String latitude, String longitude, String domain, String path, String restrict_category_ids, String include_ads) {
    Hashtable p = new Hashtable();
    p.Add("keyword",keyword);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("country",country);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("language",language);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    p.Add("include_ads",include_ads);
    return doCurl("GET","/entity/search/keyword/bynearest",p);
  }


  /**
   * Search for matching entities
   *
   *  @param what - What to get results for. E.g. Plumber e.g. plumber
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param per_page - Number of results returned per page
   *  @param page - The page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @param benchmark
   *  @return - the data from the api
  */
  public String GETEntitySearchWhat( String what, String orderonline, String delivers, String isClaimed, String per_page, String page, String country, String language, String domain, String path, String restrict_category_ids, String benchmark) {
    Hashtable p = new Hashtable();
    p.Add("what",what);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    p.Add("benchmark",benchmark);
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
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param per_page
   *  @param page
   *  @param country - A valid ISO 3166 country code e.g. ie
   *  @param language
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @return - the data from the api
  */
  public String GETEntitySearchWhatByboundingbox( String what, String latitude_1, String longitude_1, String latitude_2, String longitude_2, String orderonline, String delivers, String isClaimed, String per_page, String page, String country, String language, String domain, String path, String restrict_category_ids) {
    Hashtable p = new Hashtable();
    p.Add("what",what);
    p.Add("latitude_1",latitude_1);
    p.Add("longitude_1",longitude_1);
    p.Add("latitude_2",latitude_2);
    p.Add("longitude_2",longitude_2);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    return doCurl("GET","/entity/search/what/byboundingbox",p);
  }


  /**
   * Search for matching entities
   *
   *  @param what - What to get results for. E.g. Plumber e.g. plumber
   *  @param where - The location to get results for. E.g. Dublin e.g. Dublin
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @param latitude - The decimal latitude of the search context (optional)
   *  @param longitude - The decimal longitude of the search context (optional)
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @param benchmark
   *  @return - the data from the api
  */
  public String GETEntitySearchWhatBylocation( String what, String where, String orderonline, String delivers, String isClaimed, String per_page, String page, String country, String language, String latitude, String longitude, String domain, String path, String restrict_category_ids, String benchmark) {
    Hashtable p = new Hashtable();
    p.Add("what",what);
    p.Add("where",where);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    p.Add("benchmark",benchmark);
    return doCurl("GET","/entity/search/what/bylocation",p);
  }


  /**
   * Search for matching entities, ordered by nearness
   *
   *  @param what - What to get results for. E.g. Plumber e.g. plumber
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param country - The country to fetch results for e.g. gb
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @param language - An ISO compatible language code, E.g. en
   *  @param latitude - The decimal latitude of the centre point of the search
   *  @param longitude - The decimal longitude of the centre point of the search
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @return - the data from the api
  */
  public String GETEntitySearchWhatBynearest( String what, String orderonline, String delivers, String isClaimed, String country, String per_page, String page, String language, String latitude, String longitude, String domain, String path, String restrict_category_ids) {
    Hashtable p = new Hashtable();
    p.Add("what",what);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("country",country);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("language",language);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    return doCurl("GET","/entity/search/what/bynearest",p);
  }


  /**
   * Search for matching entities
   *
   *  @param who - Company name e.g. Starbucks
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param per_page - How many results per page
   *  @param page - What page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param language - An ISO compatible language code, E.g. en
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @param benchmark
   *  @return - the data from the api
  */
  public String GETEntitySearchWho( String who, String orderonline, String delivers, String isClaimed, String per_page, String page, String country, String language, String domain, String path, String restrict_category_ids, String benchmark) {
    Hashtable p = new Hashtable();
    p.Add("who",who);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    p.Add("benchmark",benchmark);
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
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param per_page
   *  @param page
   *  @param country
   *  @param language - An ISO compatible language code, E.g. en
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @return - the data from the api
  */
  public String GETEntitySearchWhoByboundingbox( String who, String latitude_1, String longitude_1, String latitude_2, String longitude_2, String orderonline, String delivers, String isClaimed, String per_page, String page, String country, String language, String domain, String path, String restrict_category_ids) {
    Hashtable p = new Hashtable();
    p.Add("who",who);
    p.Add("latitude_1",latitude_1);
    p.Add("longitude_1",longitude_1);
    p.Add("latitude_2",latitude_2);
    p.Add("longitude_2",longitude_2);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("language",language);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    return doCurl("GET","/entity/search/who/byboundingbox",p);
  }


  /**
   * Search for matching entities
   *
   *  @param who - Company Name e.g. Starbucks
   *  @param where - The location to get results for. E.g. Dublin e.g. Dublin
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @param country - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param latitude - The decimal latitude of the search context (optional)
   *  @param longitude - The decimal longitude of the search context (optional)
   *  @param language - An ISO compatible language code, E.g. en
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @param benchmark
   *  @return - the data from the api
  */
  public String GETEntitySearchWhoBylocation( String who, String where, String orderonline, String delivers, String isClaimed, String per_page, String page, String country, String latitude, String longitude, String language, String domain, String path, String restrict_category_ids, String benchmark) {
    Hashtable p = new Hashtable();
    p.Add("who",who);
    p.Add("where",where);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("country",country);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("language",language);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    p.Add("benchmark",benchmark);
    return doCurl("GET","/entity/search/who/bylocation",p);
  }


  /**
   * Search for entities matching the supplied 'who', ordered by nearness
   *
   *  @param who - What to get results for. E.g. Plumber e.g. plumber
   *  @param orderonline - Favours online ordering where set to true
   *  @param delivers - Favours delivery where set to true
   *  @param isClaimed - 1: claimed; 0: not claimed or claim expired; -1: ignore this filter.
   *  @param country - The country to fetch results for e.g. gb
   *  @param per_page - Number of results returned per page
   *  @param page - Which page number to retrieve
   *  @param language - An ISO compatible language code, E.g. en
   *  @param latitude - The decimal latitude of the centre point of the search
   *  @param longitude - The decimal longitude of the centre point of the search
   *  @param domain
   *  @param path
   *  @param restrict_category_ids - Pipe delimited optional IDs to restrict matches to (optional)
   *  @return - the data from the api
  */
  public String GETEntitySearchWhoBynearest( String who, String orderonline, String delivers, String isClaimed, String country, String per_page, String page, String language, String latitude, String longitude, String domain, String path, String restrict_category_ids) {
    Hashtable p = new Hashtable();
    p.Add("who",who);
    p.Add("orderonline",orderonline);
    p.Add("delivers",delivers);
    p.Add("isClaimed",isClaimed);
    p.Add("country",country);
    p.Add("per_page",per_page);
    p.Add("page",page);
    p.Add("language",language);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("domain",domain);
    p.Add("path",path);
    p.Add("restrict_category_ids",restrict_category_ids);
    return doCurl("GET","/entity/search/who/bynearest",p);
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
  public String POSTEntitySend_email( String entity_id, String gen_id, String from_email, String subject, String content) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    p.Add("from_email",from_email);
    p.Add("subject",subject);
    p.Add("content",content);
    return doCurl("POST","/entity/send_email",p);
  }


  /**
   * With a known entity id, a service object can be added.
   *
   *  @param entity_id
   *  @param value
   *  @return - the data from the api
  */
  public String POSTEntityService( String entity_id, String value) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("value",value);
    return doCurl("POST","/entity/service",p);
  }


  /**
   * With a known entity id, a service object can be deleted.
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityService( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/service",p);
  }


  /**
   * With a known entity id, a social media object can be added.
   *
   *  @param entity_id
   *  @param type
   *  @param website_url
   *  @return - the data from the api
  */
  public String POSTEntitySocialmedia( String entity_id, String type, String website_url) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("type",type);
    p.Add("website_url",website_url);
    return doCurl("POST","/entity/socialmedia",p);
  }


  /**
   * Allows a social media object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntitySocialmedia( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/socialmedia",p);
  }


  /**
   * Allows a special offer object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntitySpecial_offer( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/special_offer",p);
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
   *  @return - the data from the api
  */
  public String POSTEntitySpecial_offer( String entity_id, String title, String description, String terms, String start_date, String expiry_date, String url) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("title",title);
    p.Add("description",description);
    p.Add("terms",terms);
    p.Add("start_date",start_date);
    p.Add("expiry_date",expiry_date);
    p.Add("url",url);
    return doCurl("POST","/entity/special_offer",p);
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
  public String POSTEntityStatus( String entity_id, String status, String inactive_reason, String inactive_description) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("status",status);
    p.Add("inactive_reason",inactive_reason);
    p.Add("inactive_description",inactive_description);
    return doCurl("POST","/entity/status",p);
  }


  /**
   * Suspend all entiies added or claimed by a specific user
   *
   *  @param user_id - The unique user ID of the user with claimed entities e.g. 379236608286720
   *  @return - the data from the api
  */
  public String POSTEntityStatusSuspend_by_user_id( String user_id) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    return doCurl("POST","/entity/status/suspend_by_user_id",p);
  }


  /**
   * Allows a tag object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityTag( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/tag",p);
  }


  /**
   * With a known entity id, an tag object can be added.
   *
   *  @param entity_id
   *  @param tag
   *  @param language
   *  @return - the data from the api
  */
  public String POSTEntityTag( String entity_id, String tag, String language) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("tag",tag);
    p.Add("language",language);
    return doCurl("POST","/entity/tag",p);
  }


  /**
   * Allows a testimonial object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityTestimonial( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/testimonial",p);
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
  public String POSTEntityTestimonial( String entity_id, String title, String text, String date, String testifier_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("title",title);
    p.Add("text",text);
    p.Add("date",date);
    p.Add("testifier_name",testifier_name);
    return doCurl("POST","/entity/testimonial",p);
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
  public String GETEntityUncontribute( String entity_id, String object_name, String supplier_id, String user_id) {
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
   *  @param unmerge_masheryid
   *  @param unmerge_supplier_id
   *  @param unmerge_user_id
   *  @param destructive
   *  @return - the data from the api
  */
  public String POSTEntityUnmerge( String entity_id, String unmerge_masheryid, String unmerge_supplier_id, String unmerge_user_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("unmerge_masheryid",unmerge_masheryid);
    p.Add("unmerge_supplier_id",unmerge_supplier_id);
    p.Add("unmerge_user_id",unmerge_user_id);
    p.Add("destructive",destructive);
    return doCurl("POST","/entity/unmerge",p);
  }


  /**
   * Find the provided user in all the sub objects and update the trust
   *
   *  @param entity_id - the entity_id to update
   *  @param user_id - the user to search for
   *  @param trust - The new trust for the user
   *  @return - the data from the api
  */
  public String POSTEntityUser_trust( String entity_id, String user_id, String trust) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("user_id",user_id);
    p.Add("trust",trust);
    return doCurl("POST","/entity/user_trust",p);
  }


  /**
   * Add a verified source object to an existing entity.
   *
   *  @param entity_id
   *  @param public_source - Corresponds to entity_obj.attribution.name
   *  @param source_name - Corresponds to entity_obj.data_sources.type
   *  @param source_id - Corresponds to entity_obj.data_sources.external_id
   *  @param source_url - Corresponds to entity_obj.attribution.url
   *  @param source_logo - Corresponds to entity_obj.attribution.logo
   *  @param verified_date - Corresponds to entity_obj.data_sources.created_at
   *  @return - the data from the api
  */
  public String POSTEntityVerified( String entity_id, String public_source, String source_name, String source_id, String source_url, String source_logo, String verified_date) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("public_source",public_source);
    p.Add("source_name",source_name);
    p.Add("source_id",source_id);
    p.Add("source_url",source_url);
    p.Add("source_logo",source_logo);
    p.Add("verified_date",verified_date);
    return doCurl("POST","/entity/verified",p);
  }


  /**
   * Remove a verified source object to an existing entity.
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String DELETEEntityVerified( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/entity/verified",p);
  }


  /**
   * With a known entity id, a video object can be added.
   *
   *  @param entity_id
   *  @param type
   *  @param link
   *  @return - the data from the api
  */
  public String POSTEntityVideo( String entity_id, String type, String link) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("type",type);
    p.Add("link",link);
    return doCurl("POST","/entity/video",p);
  }


  /**
   * Allows a video object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityVideo( String entity_id, String gen_id) {
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
  public String POSTEntityVideoYoutube( String entity_id, String embed_code) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("embed_code",embed_code);
    return doCurl("POST","/entity/video/youtube",p);
  }


  /**
   * Allows a website object to be reduced in confidence
   *
   *  @param entity_id
   *  @param gen_id
   *  @param force
   *  @return - the data from the api
  */
  public String DELETEEntityWebsite( String entity_id, String gen_id, String force) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    p.Add("force",force);
    return doCurl("DELETE","/entity/website",p);
  }


  /**
   * With a known entity id, a website object can be added.
   *
   *  @param entity_id
   *  @param website_url
   *  @param display_url
   *  @param website_description
   *  @param gen_id
   *  @return - the data from the api
  */
  public String POSTEntityWebsite( String entity_id, String website_url, String display_url, String website_description, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("website_url",website_url);
    p.Add("display_url",display_url);
    p.Add("website_description",website_description);
    p.Add("gen_id",gen_id);
    return doCurl("POST","/entity/website",p);
  }


  /**
   * With a known entity id, a yext list can be added
   *
   *  @param entity_id
   *  @param yext_list_id
   *  @param description
   *  @param name
   *  @param type
   *  @return - the data from the api
  */
  public String POSTEntityYext_list( String entity_id, String yext_list_id, String description, String name, String type) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("yext_list_id",yext_list_id);
    p.Add("description",description);
    p.Add("name",name);
    p.Add("type",type);
    return doCurl("POST","/entity/yext_list",p);
  }


  /**
   * Allows a yext list object to be removed
   *
   *  @param entity_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEEntityYext_list( String entity_id, String gen_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("gen_id",gen_id);
    return doCurl("DELETE","/entity/yext_list",p);
  }


  /**
   * Add an entityserve document
   *
   *  @param entity_id - The ids of the entity/entities to create the entityserve event(s) for
   *  @param country - the ISO code of the country
   *  @param event_type - The event type being recorded
   *  @param domain
   *  @param path
   *  @return - the data from the api
  */
  public String PUTEntityserve( String entity_id, String country, String event_type, String domain, String path) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("country",country);
    p.Add("event_type",event_type);
    p.Add("domain",domain);
    p.Add("path",path);
    return doCurl("PUT","/entityserve",p);
  }


  /**
   * Update/Add a flatpack
   *
   *  @param flatpack_id - this record's unique, auto-generated id - if supplied, then this is an edit, otherwise it's an add
   *  @param status - The status of the flatpack, it is required on creation. Syndication link logic depends on this.
   *  @param nodefaults - create an flatpack that's empty apart from provided values (used for child flatpacks), IMPORTANT: if set, any parameters with default values will be ignored even if overridden. Edit the created flatpack to set those parameters on a nodefaults flatpack.
   *  @param domainName - the domain name to serve this flatpack site on (no leading http:// or anything please)
   *  @param inherits - inherit from domain
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
   *  @param serpAdsBottom - The block of HTML/JS that renders Ads on Serps at the bottom
   *  @param serpTitleNoWhat - The text to display in the title for where only searches
   *  @param serpDescriptionNoWhat - The text to display in the description for where only searches
   *  @param cookiePolicyUrl - The cookie policy url of the flatpack
   *  @param cookiePolicyNotice - Whether to show the cookie policy on this flatpack
   *  @param addBusinessButtonText - The text used in the 'Add your business' button
   *  @param twitterUrl - Twitter link
   *  @param facebookUrl - Facebook link
   *  @param copyright - Copyright message
   *  @param phoneReveal - record phone number reveal
   *  @param loginLinkText - the link text for the Login link
   *  @param contextLocationId - The location ID to use as the context for searches on this flatpack
   *  @param addBusinessButtonPosition - The location ID to use as the context for searches on this flatpack
   *  @param denyIndexing - Whether to noindex a flatpack
   *  @param contextRadius - allows you to set a catchment area around the contextLocationId in miles for use when displaying the activity stream module
   *  @param activityStream - allows you to set the activity to be displayed in the activity stream
   *  @param activityStreamSize - Sets the number of items to show within the activity stream.
   *  @param products - A Collection of Central Index products the flatpack is allowed to sell
   *  @param linkToRoot - The root domain name to serve this flatpack site on (no leading http:// or anything please)
   *  @param termsLink - A URL for t's and c's specific to this partner
   *  @param serpNumberEmbedAdverts - The number of embed adverts per search
   *  @param serpEmbedTitle - Custom page title for emdedded searches
   *  @param adminLess - the LESS configuration to use to overrides the Bootstrap CSS for the admin on themed domains
   *  @param adminConfNoLocz - operate without recourse to verified location data (locz)
   *  @param adminConfNoSocialLogin - suppress social media login interface
   *  @param adminConfEasyClaim - captcha only for claim
   *  @param adminConfPaymentMode - payment gateway
   *  @param adminConfEnableProducts - show upgrade on claim
   *  @param adminConfSimpleadmin - render a template for the reduced functionality
   *  @return - the data from the api
  */
  public String POSTFlatpack( String flatpack_id, String status, String nodefaults, String domainName, String inherits, String stub, String flatpackName, String less, String language, String country, String mapsType, String mapKey, String searchFormShowOn, String searchFormShowKeywordsBox, String searchFormShowLocationBox, String searchFormKeywordsAutoComplete, String searchFormLocationsAutoComplete, String searchFormDefaultLocation, String searchFormPlaceholderKeywords, String searchFormPlaceholderLocation, String searchFormKeywordsLabel, String searchFormLocationLabel, String cannedLinksHeader, String homepageTitle, String homepageDescription, String homepageIntroTitle, String homepageIntroText, String head, String adblock, String bodyTop, String bodyBottom, String header_menu, String header_menu_bottom, String footer_menu, String bdpTitle, String bdpDescription, String bdpAds, String serpTitle, String serpDescription, String serpNumberResults, String serpNumberAdverts, String serpAds, String serpAdsBottom, String serpTitleNoWhat, String serpDescriptionNoWhat, String cookiePolicyUrl, String cookiePolicyNotice, String addBusinessButtonText, String twitterUrl, String facebookUrl, String copyright, String phoneReveal, String loginLinkText, String contextLocationId, String addBusinessButtonPosition, String denyIndexing, String contextRadius, String activityStream, String activityStreamSize, String products, String linkToRoot, String termsLink, String serpNumberEmbedAdverts, String serpEmbedTitle, String adminLess, String adminConfNoLocz, String adminConfNoSocialLogin, String adminConfEasyClaim, String adminConfPaymentMode, String adminConfEnableProducts, String adminConfSimpleadmin) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("status",status);
    p.Add("nodefaults",nodefaults);
    p.Add("domainName",domainName);
    p.Add("inherits",inherits);
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
    p.Add("serpAdsBottom",serpAdsBottom);
    p.Add("serpTitleNoWhat",serpTitleNoWhat);
    p.Add("serpDescriptionNoWhat",serpDescriptionNoWhat);
    p.Add("cookiePolicyUrl",cookiePolicyUrl);
    p.Add("cookiePolicyNotice",cookiePolicyNotice);
    p.Add("addBusinessButtonText",addBusinessButtonText);
    p.Add("twitterUrl",twitterUrl);
    p.Add("facebookUrl",facebookUrl);
    p.Add("copyright",copyright);
    p.Add("phoneReveal",phoneReveal);
    p.Add("loginLinkText",loginLinkText);
    p.Add("contextLocationId",contextLocationId);
    p.Add("addBusinessButtonPosition",addBusinessButtonPosition);
    p.Add("denyIndexing",denyIndexing);
    p.Add("contextRadius",contextRadius);
    p.Add("activityStream",activityStream);
    p.Add("activityStreamSize",activityStreamSize);
    p.Add("products",products);
    p.Add("linkToRoot",linkToRoot);
    p.Add("termsLink",termsLink);
    p.Add("serpNumberEmbedAdverts",serpNumberEmbedAdverts);
    p.Add("serpEmbedTitle",serpEmbedTitle);
    p.Add("adminLess",adminLess);
    p.Add("adminConfNoLocz",adminConfNoLocz);
    p.Add("adminConfNoSocialLogin",adminConfNoSocialLogin);
    p.Add("adminConfEasyClaim",adminConfEasyClaim);
    p.Add("adminConfPaymentMode",adminConfPaymentMode);
    p.Add("adminConfEnableProducts",adminConfEnableProducts);
    p.Add("adminConfSimpleadmin",adminConfSimpleadmin);
    return doCurl("POST","/flatpack",p);
  }


  /**
   * Get a flatpack
   *
   *  @param flatpack_id - the unique id to search for
   *  @return - the data from the api
  */
  public String GETFlatpack( String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/flatpack",p);
  }


  /**
   * Remove a flatpack using a supplied flatpack_id
   *
   *  @param flatpack_id - the id of the flatpack to delete
   *  @return - the data from the api
  */
  public String DELETEFlatpack( String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    return doCurl("DELETE","/flatpack",p);
  }


  /**
   * Upload a CSS file for the Admin for this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTFlatpackAdminCSS( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/adminCSS",p);
  }


  /**
   * Add a HD Admin logo to a flatpack domain
   *
   *  @param flatpack_id - the unique id to search for
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTFlatpackAdminHDLogo( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/adminHDLogo",p);
  }


  /**
   * Upload an image to serve out as the large logo in the Admin for this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTFlatpackAdminLargeLogo( String flatpack_id, String filedata) {
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
  public String POSTFlatpackAdminSmallLogo( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/adminSmallLogo",p);
  }


  /**
   * Remove a flatpack using a supplied flatpack_id
   *
   *  @param flatpack_id - the unique id to search for
   *  @param adblock
   *  @param serpAds
   *  @param serpAdsBottom
   *  @param bdpAds
   *  @return - the data from the api
  */
  public String DELETEFlatpackAds( String flatpack_id, String adblock, String serpAds, String serpAdsBottom, String bdpAds) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("adblock",adblock);
    p.Add("serpAds",serpAds);
    p.Add("serpAdsBottom",serpAdsBottom);
    p.Add("bdpAds",bdpAds);
    return doCurl("DELETE","/flatpack/ads",p);
  }


  /**
   * Generate flatpacks based on the files passed in
   *
   *  @param json - The flatpack JSON to make replacements on
   *  @param filedata - a file that contains the replacements in the JSON
   *  @param slack_user
   *  @return - the data from the api
  */
  public String POSTFlatpackBulkJson( String json, String filedata, String slack_user) {
    Hashtable p = new Hashtable();
    p.Add("json",json);
    p.Add("filedata",filedata);
    p.Add("slack_user",slack_user);
    return doCurl("POST","/flatpack/bulk/json",p);
  }


  /**
   * Get flatpacks by country and location
   *
   *  @param country
   *  @param latitude
   *  @param longitude
   *  @return - the data from the api
  */
  public String GETFlatpackBy_country( String country, String latitude, String longitude) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    return doCurl("GET","/flatpack/by_country",p);
  }


  /**
   * Get flatpacks by country in KML format
   *
   *  @param country
   *  @return - the data from the api
  */
  public String GETFlatpackBy_countryKml( String country) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    return doCurl("GET","/flatpack/by_country/kml",p);
  }


  /**
   * Get a flatpack using a domain name
   *
   *  @param domainName - the domain name to search for
   *  @param matchAlias - Whether to match alias as well
   *  @return - the data from the api
  */
  public String GETFlatpackBy_domain_name( String domainName, String matchAlias) {
    Hashtable p = new Hashtable();
    p.Add("domainName",domainName);
    p.Add("matchAlias",matchAlias);
    return doCurl("GET","/flatpack/by_domain_name",p);
  }


  /**
   * Get flatpacks that match the supplied masheryid
   *
   *  @return - the data from the api
  */
  public String GETFlatpackBy_masheryid() {
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
  public String GETFlatpackClone( String flatpack_id, String domainName) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("domainName",domainName);
    return doCurl("GET","/flatpack/clone",p);
  }


  /**
   * undefined
   *
   *  @param flatpack_id - the unique id to search for
   *  @param domainName
   *  @return - the data from the api
  */
  public String POSTFlatpackDomain_alias( String flatpack_id, String domainName) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("domainName",domainName);
    return doCurl("POST","/flatpack/domain_alias",p);
  }


  /**
   * undefined
   *
   *  @param flatpack_id - the unique id to search for
   *  @param domainName
   *  @return - the data from the api
  */
  public String DELETEFlatpackDomain_alias( String flatpack_id, String domainName) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("domainName",domainName);
    return doCurl("DELETE","/flatpack/domain_alias",p);
  }


  /**
   * Returns a list of domain names in which direct/inherited flatpack country match the specified one and status equals production.
   *
   *  @param country
   *  @return - the data from the api
  */
  public String GETFlatpackDomain_nameBy_country( String country) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    return doCurl("GET","/flatpack/domain_name/by_country",p);
  }


  /**
   * Upload an icon to serve out with this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTFlatpackIcon( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/icon",p);
  }


  /**
   * Get a flatpack using a domain name
   *
   *  @param flatpack_id - the id to search for
   *  @return - the data from the api
  */
  public String GETFlatpackInherit( String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/flatpack/inherit",p);
  }


  /**
   * returns the LESS theme from a flatpack
   *
   *  @param flatpack_id - the unique id to search for
   *  @return - the data from the api
  */
  public String GETFlatpackLess( String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/flatpack/less",p);
  }


  /**
   * Remove a canned link to an existing flatpack site.
   *
   *  @param flatpack_id - the id of the flatpack to delete
   *  @param gen_id - the id of the canned link to remove
   *  @return - the data from the api
  */
  public String DELETEFlatpackLink( String flatpack_id, String gen_id) {
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
  public String POSTFlatpackLink( String flatpack_id, String keywords, String location, String linkText) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("keywords",keywords);
    p.Add("location",location);
    p.Add("linkText",linkText);
    return doCurl("POST","/flatpack/link",p);
  }


  /**
   * Remove all canned links from an existing flatpack.
   *
   *  @param flatpack_id - the id of the flatpack to remove links from
   *  @return - the data from the api
  */
  public String DELETEFlatpackLinkAll( String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    return doCurl("DELETE","/flatpack/link/all",p);
  }


  /**
   * Upload a logo to serve out with this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTFlatpackLogo( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/logo",p);
  }


  /**
   * Add a hd logo to a flatpack domain
   *
   *  @param flatpack_id - the unique id to search for
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTFlatpackLogoHd( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/logo/hd",p);
  }


  /**
   * Delete a Redirect link from a flatpack
   *
   *  @param flatpack_id - the unique id to search for
   *  @return - the data from the api
  */
  public String DELETEFlatpackRedirect( String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    return doCurl("DELETE","/flatpack/redirect",p);
  }


  /**
   * Add a Redirect link to a flatpack
   *
   *  @param flatpack_id - the unique id to search for
   *  @param redirectTo
   *  @return - the data from the api
  */
  public String POSTFlatpackRedirect( String flatpack_id, String redirectTo) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("redirectTo",redirectTo);
    return doCurl("POST","/flatpack/redirect",p);
  }


  /**
   * Upload a TXT file to act as the sitemap for this flatpack
   *
   *  @param flatpack_id - the id of the flatpack to update
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTFlatpackSitemap( String flatpack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("flatpack_id",flatpack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/flatpack/sitemap",p);
  }


  /**
   * Delete a group with a specified group_id
   *
   *  @param group_id
   *  @return - the data from the api
  */
  public String DELETEGroup( String group_id) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    return doCurl("DELETE","/group",p);
  }


  /**
   * Update/Add a Group
   *
   *  @param group_id
   *  @param name
   *  @param description
   *  @param url
   *  @param stamp_user_id
   *  @param stamp_sql
   *  @return - the data from the api
  */
  public String POSTGroup( String group_id, String name, String description, String url, String stamp_user_id, String stamp_sql) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    p.Add("name",name);
    p.Add("description",description);
    p.Add("url",url);
    p.Add("stamp_user_id",stamp_user_id);
    p.Add("stamp_sql",stamp_sql);
    return doCurl("POST","/group",p);
  }


  /**
   * Returns group that matches a given group id
   *
   *  @param group_id
   *  @return - the data from the api
  */
  public String GETGroup( String group_id) {
    Hashtable p = new Hashtable();
    p.Add("group_id",group_id);
    return doCurl("GET","/group",p);
  }


  /**
   * Returns all groups
   *
   *  @return - the data from the api
  */
  public String GETGroupAll() {
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
  public String POSTGroupBulk_delete( String group_id, String filedata) {
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
  public String POSTGroupBulk_ingest( String group_id, String filedata, String category_type) {
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
  public String POSTGroupBulk_update( String group_id, String data) {
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
  public String GETHeartbeatBy_date( String from_date, String to_date, String country_id) {
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
  public String GETHeartbeatTodayClaims( String country, String claim_type) {
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
  public String POSTIngest_file( String job_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("job_id",job_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/ingest_file",p);
  }


  /**
   * Add a ingest job to the collection
   *
   *  @param description
   *  @param category_type
   *  @return - the data from the api
  */
  public String POSTIngest_job( String description, String category_type) {
    Hashtable p = new Hashtable();
    p.Add("description",description);
    p.Add("category_type",category_type);
    return doCurl("POST","/ingest_job",p);
  }


  /**
   * Get an ingest job from the collection
   *
   *  @param job_id
   *  @return - the data from the api
  */
  public String GETIngest_job( String job_id) {
    Hashtable p = new Hashtable();
    p.Add("job_id",job_id);
    return doCurl("GET","/ingest_job",p);
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
  public String GETIngest_logBy_job_id( String job_id, String success, String errors, String limit, String skip) {
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
  public String GETIngest_queue( String flush) {
    Hashtable p = new Hashtable();
    p.Add("flush",flush);
    return doCurl("GET","/ingest_queue",p);
  }


  /**
   * Returns entities that do not have claim or advertisers data
   *
   *  @param country_id - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param from_date
   *  @param to_date
   *  @param limit
   *  @param offset
   *  @param reduce - Set true to return the count value only.
   *  @return - the data from the api
  */
  public String GETLeadsAdded( String country_id, String from_date, String to_date, String limit, String offset, String reduce) {
    Hashtable p = new Hashtable();
    p.Add("country_id",country_id);
    p.Add("from_date",from_date);
    p.Add("to_date",to_date);
    p.Add("limit",limit);
    p.Add("offset",offset);
    p.Add("reduce",reduce);
    return doCurl("GET","/leads/added",p);
  }


  /**
   * Returns entities that have advertisers data
   *
   *  @param country_id - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param from_date
   *  @param to_date
   *  @param limit
   *  @param offset
   *  @param reduce - Set true to return the count value only.
   *  @return - the data from the api
  */
  public String GETLeadsAdvertisers( String country_id, String from_date, String to_date, String limit, String offset, String reduce) {
    Hashtable p = new Hashtable();
    p.Add("country_id",country_id);
    p.Add("from_date",from_date);
    p.Add("to_date",to_date);
    p.Add("limit",limit);
    p.Add("offset",offset);
    p.Add("reduce",reduce);
    return doCurl("GET","/leads/advertisers",p);
  }


  /**
   * Returns entities that have claim data
   *
   *  @param country_id - Which country to return results for. An ISO compatible country code, E.g. ie e.g. ie
   *  @param from_date
   *  @param to_date
   *  @param limit
   *  @param offset
   *  @param reduce - Set true to return the count value only.
   *  @return - the data from the api
  */
  public String GETLeadsClaimed( String country_id, String from_date, String to_date, String limit, String offset, String reduce) {
    Hashtable p = new Hashtable();
    p.Add("country_id",country_id);
    p.Add("from_date",from_date);
    p.Add("to_date",to_date);
    p.Add("limit",limit);
    p.Add("offset",offset);
    p.Add("reduce",reduce);
    return doCurl("GET","/leads/claimed",p);
  }


  /**
   * Read a location with the supplied ID in the locations reference database.
   *
   *  @param location_id
   *  @return - the data from the api
  */
  public String GETLocation( String location_id) {
    Hashtable p = new Hashtable();
    p.Add("location_id",location_id);
    return doCurl("GET","/location",p);
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
  public String POSTLocation( String location_id, String type, String country, String language, String name, String formal_name, String resolution, String population, String description, String timezone, String latitude, String longitude, String parent_town, String parent_county, String parent_province, String parent_region, String parent_neighbourhood, String parent_district, String postalcode, String searchable_id, String searchable_ids) {
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
   * Given a location_id or a lat/lon, find other locations within the radius
   *
   *  @param location_id
   *  @param latitude
   *  @param longitude
   *  @param radius - Radius in km
   *  @param resolution
   *  @param country
   *  @param num_results
   *  @return - the data from the api
  */
  public String GETLocationContext( String location_id, String latitude, String longitude, String radius, String resolution, String country, String num_results) {
    Hashtable p = new Hashtable();
    p.Add("location_id",location_id);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    p.Add("radius",radius);
    p.Add("resolution",resolution);
    p.Add("country",country);
    p.Add("num_results",num_results);
    return doCurl("GET","/location/context",p);
  }


  /**
   * Read multiple locations with the supplied ID in the locations reference database.
   *
   *  @param location_ids
   *  @return - the data from the api
  */
  public String GETLocationMultiple( String location_ids) {
    Hashtable p = new Hashtable();
    p.Add("location_ids",location_ids);
    return doCurl("GET","/location/multiple",p);
  }


  /**
   * With a unique login_id a login can be retrieved
   *
   *  @param login_id
   *  @return - the data from the api
  */
  public String GETLogin( String login_id) {
    Hashtable p = new Hashtable();
    p.Add("login_id",login_id);
    return doCurl("GET","/login",p);
  }


  /**
   * Create/Update login details
   *
   *  @param login_id
   *  @param email
   *  @param password
   *  @return - the data from the api
  */
  public String POSTLogin( String login_id, String email, String password) {
    Hashtable p = new Hashtable();
    p.Add("login_id",login_id);
    p.Add("email",email);
    p.Add("password",password);
    return doCurl("POST","/login",p);
  }


  /**
   * With a unique login_id a login can be deleted
   *
   *  @param login_id
   *  @return - the data from the api
  */
  public String DELETELogin( String login_id) {
    Hashtable p = new Hashtable();
    p.Add("login_id",login_id);
    return doCurl("DELETE","/login",p);
  }


  /**
   * With a unique email address a login can be retrieved
   *
   *  @param email
   *  @return - the data from the api
  */
  public String GETLoginBy_email( String email) {
    Hashtable p = new Hashtable();
    p.Add("email",email);
    return doCurl("GET","/login/by_email",p);
  }


  /**
   * Verify that a supplied email and password match a users saved login details
   *
   *  @param email
   *  @param password
   *  @return - the data from the api
  */
  public String GETLoginVerify( String email, String password) {
    Hashtable p = new Hashtable();
    p.Add("email",email);
    p.Add("password",password);
    return doCurl("GET","/login/verify",p);
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
  public String GETLogo( String a, String b, String c, String d) {
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
  public String PUTLogo( String a) {
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
  public String GETLookupCategory( String _string, String language) {
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
  public String GETLookupLegacyCategory( String id, String type) {
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
  public String GETLookupLocation( String _string, String language, String country, String latitude, String longitude) {
    Hashtable p = new Hashtable();
    p.Add("string",_string);
    p.Add("language",language);
    p.Add("country",country);
    p.Add("latitude",latitude);
    p.Add("longitude",longitude);
    return doCurl("GET","/lookup/location",p);
  }


  /**
   * Returns a list of mashery IDs domain names in which direct/inherited flatpack country match the specified one and status equals production.
   *
   *  @return - the data from the api
  */
  public String GETMasheryidAll() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/masheryid/all",p);
  }


  /**
   * Find all matches by phone number, returning up to 10 matches
   *
   *  @param phone
   *  @param country
   *  @param exclude - Entity ID to exclude from the results
   *  @return - the data from the api
  */
  public String GETMatchByphone( String phone, String country, String exclude) {
    Hashtable p = new Hashtable();
    p.Add("phone",phone);
    p.Add("country",country);
    p.Add("exclude",exclude);
    return doCurl("GET","/match/byphone",p);
  }


  /**
   * Perform a match on the two supplied entities, returning the outcome and showing our working
   *
   *  @param primary_entity_id
   *  @param secondary_entity_id
   *  @param return_entities - Should we return the entity documents
   *  @return - the data from the api
  */
  public String GETMatchOftheday( String primary_entity_id, String secondary_entity_id, String return_entities) {
    Hashtable p = new Hashtable();
    p.Add("primary_entity_id",primary_entity_id);
    p.Add("secondary_entity_id",secondary_entity_id);
    p.Add("return_entities",return_entities);
    return doCurl("GET","/match/oftheday",p);
  }


  /**
   * Will create a new Matching Instruction or update an existing one
   *
   *  @param entity_id
   *  @param entity_name
   *  @return - the data from the api
  */
  public String POSTMatching_instruction( String entity_id, String entity_name) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("entity_name",entity_name);
    return doCurl("POST","/matching_instruction",p);
  }


  /**
   * Delete Matching instruction
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String DELETEMatching_instruction( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("DELETE","/matching_instruction",p);
  }


  /**
   * Fetch all available Matching instructions
   *
   *  @param limit
   *  @return - the data from the api
  */
  public String GETMatching_instructionAll( String limit) {
    Hashtable p = new Hashtable();
    p.Add("limit",limit);
    return doCurl("GET","/matching_instruction/all",p);
  }


  /**
   * Create a matching log
   *
   *  @param primary_entity_id
   *  @param secondary_entity_id
   *  @param primary_name
   *  @param secondary_name
   *  @param address_score
   *  @param address_match
   *  @param name_score
   *  @param name_match
   *  @param distance
   *  @param phone_match
   *  @param category_match
   *  @param email_match
   *  @param website_match
   *  @param match
   *  @return - the data from the api
  */
  public String PUTMatching_log( String primary_entity_id, String secondary_entity_id, String primary_name, String secondary_name, String address_score, String address_match, String name_score, String name_match, String distance, String phone_match, String category_match, String email_match, String website_match, String match) {
    Hashtable p = new Hashtable();
    p.Add("primary_entity_id",primary_entity_id);
    p.Add("secondary_entity_id",secondary_entity_id);
    p.Add("primary_name",primary_name);
    p.Add("secondary_name",secondary_name);
    p.Add("address_score",address_score);
    p.Add("address_match",address_match);
    p.Add("name_score",name_score);
    p.Add("name_match",name_match);
    p.Add("distance",distance);
    p.Add("phone_match",phone_match);
    p.Add("category_match",category_match);
    p.Add("email_match",email_match);
    p.Add("website_match",website_match);
    p.Add("match",match);
    return doCurl("PUT","/matching_log",p);
  }


  /**
   * With a known user ID add/create the maxclaims blcok
   *
   *  @param user_id
   *  @param contract_id
   *  @param country
   *  @param number
   *  @param expiry_date
   *  @return - the data from the api
  */
  public String POSTMaxclaimsActivate( String user_id, String contract_id, String country, String number, String expiry_date) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    p.Add("contract_id",contract_id);
    p.Add("country",country);
    p.Add("number",number);
    p.Add("expiry_date",expiry_date);
    return doCurl("POST","/maxclaims/activate",p);
  }


  /**
   * Fetching a message
   *
   *  @param message_id - The message id to pull the message for
   *  @return - the data from the api
  */
  public String GETMessage( String message_id) {
    Hashtable p = new Hashtable();
    p.Add("message_id",message_id);
    return doCurl("GET","/message",p);
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
  public String POSTMessage( String message_id, String ses_id, String from_user_id, String from_email, String to_entity_id, String to_email, String subject, String body, String bounced) {
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
   * Fetching messages by ses_id
   *
   *  @param ses_id - The amazon id to pull the message for
   *  @return - the data from the api
  */
  public String GETMessageBy_ses_id( String ses_id) {
    Hashtable p = new Hashtable();
    p.Add("ses_id",ses_id);
    return doCurl("GET","/message/by_ses_id",p);
  }


  /**
   * Update/Add a multipack
   *
   *  @param multipack_id - this record's unique, auto-generated id - if supplied, then this is an edit, otherwise it's an add
   *  @param group_id - the id of the group that this site serves
   *  @param domainName - the domain name to serve this multipack site on (no leading http:// or anything please)
   *  @param multipackName - the name of the Flat pack instance
   *  @param less - the LESS configuration to use to overrides the Bootstrap CSS
   *  @param country - the country to use for searches etc
   *  @param menuTop - the JSON that describes a navigation at the top of the page
   *  @param menuBottom - the JSON that describes a navigation below the masthead
   *  @param language - An ISO compatible language code, E.g. en e.g. en
   *  @param menuFooter - the JSON that describes a navigation at the bottom of the page
   *  @param searchNumberResults - the number of search results per page
   *  @param searchTitle - Title of serps page
   *  @param searchDescription - Description of serps page
   *  @param searchTitleNoWhere - Title when no where is specified
   *  @param searchDescriptionNoWhere - Description of serps page when no where is specified
   *  @param searchIntroHeader - Introductory header
   *  @param searchIntroText - Introductory text
   *  @param searchShowAll - display all search results on one page
   *  @param searchUnitOfDistance - the unit of distance to use for search
   *  @param cookiePolicyShow - whether to show cookie policy
   *  @param cookiePolicyUrl - url of cookie policy
   *  @param twitterUrl - url of twitter feed
   *  @param facebookUrl - url of facebook feed
   *  @return - the data from the api
  */
  public String POSTMultipack( String multipack_id, String group_id, String domainName, String multipackName, String less, String country, String menuTop, String menuBottom, String language, String menuFooter, String searchNumberResults, String searchTitle, String searchDescription, String searchTitleNoWhere, String searchDescriptionNoWhere, String searchIntroHeader, String searchIntroText, String searchShowAll, String searchUnitOfDistance, String cookiePolicyShow, String cookiePolicyUrl, String twitterUrl, String facebookUrl) {
    Hashtable p = new Hashtable();
    p.Add("multipack_id",multipack_id);
    p.Add("group_id",group_id);
    p.Add("domainName",domainName);
    p.Add("multipackName",multipackName);
    p.Add("less",less);
    p.Add("country",country);
    p.Add("menuTop",menuTop);
    p.Add("menuBottom",menuBottom);
    p.Add("language",language);
    p.Add("menuFooter",menuFooter);
    p.Add("searchNumberResults",searchNumberResults);
    p.Add("searchTitle",searchTitle);
    p.Add("searchDescription",searchDescription);
    p.Add("searchTitleNoWhere",searchTitleNoWhere);
    p.Add("searchDescriptionNoWhere",searchDescriptionNoWhere);
    p.Add("searchIntroHeader",searchIntroHeader);
    p.Add("searchIntroText",searchIntroText);
    p.Add("searchShowAll",searchShowAll);
    p.Add("searchUnitOfDistance",searchUnitOfDistance);
    p.Add("cookiePolicyShow",cookiePolicyShow);
    p.Add("cookiePolicyUrl",cookiePolicyUrl);
    p.Add("twitterUrl",twitterUrl);
    p.Add("facebookUrl",facebookUrl);
    return doCurl("POST","/multipack",p);
  }


  /**
   * Get a multipack
   *
   *  @param multipack_id - the unique id to search for
   *  @return - the data from the api
  */
  public String GETMultipack( String multipack_id) {
    Hashtable p = new Hashtable();
    p.Add("multipack_id",multipack_id);
    return doCurl("GET","/multipack",p);
  }


  /**
   * Add an admin theme to a multipack
   *
   *  @param multipack_id - the unique id to search for
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTMultipackAdminCSS( String multipack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("multipack_id",multipack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/multipack/adminCSS",p);
  }


  /**
   * Add a Admin logo to a Multipack domain
   *
   *  @param multipack_id - the unique id to search for
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTMultipackAdminLogo( String multipack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("multipack_id",multipack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/multipack/adminLogo",p);
  }


  /**
   * Get a multipack using a domain name
   *
   *  @param domainName - the domain name to search for
   *  @return - the data from the api
  */
  public String GETMultipackBy_domain_name( String domainName) {
    Hashtable p = new Hashtable();
    p.Add("domainName",domainName);
    return doCurl("GET","/multipack/by_domain_name",p);
  }


  /**
   * duplicates a given multipack
   *
   *  @param multipack_id - the unique id to search for
   *  @param domainName - the domain name to serve this multipack site on (no leading http:// or anything please)
   *  @param group_id - the group to use for search
   *  @return - the data from the api
  */
  public String GETMultipackClone( String multipack_id, String domainName, String group_id) {
    Hashtable p = new Hashtable();
    p.Add("multipack_id",multipack_id);
    p.Add("domainName",domainName);
    p.Add("group_id",group_id);
    return doCurl("GET","/multipack/clone",p);
  }


  /**
   * returns the LESS theme from a multipack
   *
   *  @param multipack_id - the unique id to search for
   *  @return - the data from the api
  */
  public String GETMultipackLess( String multipack_id) {
    Hashtable p = new Hashtable();
    p.Add("multipack_id",multipack_id);
    return doCurl("GET","/multipack/less",p);
  }


  /**
   * Add a logo to a multipack domain
   *
   *  @param multipack_id - the unique id to search for
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTMultipackLogo( String multipack_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("multipack_id",multipack_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/multipack/logo",p);
  }


  /**
   * Add a map pin to a multipack domain
   *
   *  @param multipack_id - the unique id to search for
   *  @param filedata
   *  @param mapPinOffsetX
   *  @param mapPinOffsetY
   *  @return - the data from the api
  */
  public String POSTMultipackMap_pin( String multipack_id, String filedata, String mapPinOffsetX, String mapPinOffsetY) {
    Hashtable p = new Hashtable();
    p.Add("multipack_id",multipack_id);
    p.Add("filedata",filedata);
    p.Add("mapPinOffsetX",mapPinOffsetX);
    p.Add("mapPinOffsetY",mapPinOffsetY);
    return doCurl("POST","/multipack/map_pin",p);
  }


  /**
   * Fetch an ops_log
   *
   *  @param ops_log_id
   *  @return - the data from the api
  */
  public String GETOps_log( String ops_log_id) {
    Hashtable p = new Hashtable();
    p.Add("ops_log_id",ops_log_id);
    return doCurl("GET","/ops_log",p);
  }


  /**
   * Create an ops_log
   *
   *  @param success
   *  @param type
   *  @param action
   *  @param data
   *  @param slack_channel
   *  @return - the data from the api
  */
  public String POSTOps_log( String success, String type, String action, String data, String slack_channel) {
    Hashtable p = new Hashtable();
    p.Add("success",success);
    p.Add("type",type);
    p.Add("action",action);
    p.Add("data",data);
    p.Add("slack_channel",slack_channel);
    return doCurl("POST","/ops_log",p);
  }


  /**
   * Run PTB for a given ingest job ID.
   *
   *  @param ingest_job_id - The ingest job ID
   *  @param email - When all entity IDs are pushed to the PTB queue, an email containing debug info will be sent.
   *  @return - the data from the api
  */
  public String POSTPaintBy_ingest_job_id( String ingest_job_id, String email) {
    Hashtable p = new Hashtable();
    p.Add("ingest_job_id",ingest_job_id);
    p.Add("email",email);
    return doCurl("POST","/paint/by_ingest_job_id",p);
  }


  /**
   * With a known entity id syndication of data back to a partner is enabled
   *
   *  @param entity_id
   *  @param publisher_id
   *  @param expiry_date
   *  @return - the data from the api
  */
  public String POSTPartnersyndicateActivate( String entity_id, String publisher_id, String expiry_date) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("publisher_id",publisher_id);
    p.Add("expiry_date",expiry_date);
    return doCurl("POST","/partnersyndicate/activate",p);
  }


  /**
   * Call CK syndication instruction and call cancel endpoint for partner/supplier_id
   *
   *  @param supplierid
   *  @param vendor
   *  @return - the data from the api
  */
  public String POSTPartnersyndicateCancel( String supplierid, String vendor) {
    Hashtable p = new Hashtable();
    p.Add("supplierid",supplierid);
    p.Add("vendor",vendor);
    return doCurl("POST","/partnersyndicate/cancel",p);
  }


  /**
   * This will call into CK in order to create the entity on the third party system.
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String POSTPartnersyndicateCreate( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("POST","/partnersyndicate/create",p);
  }


  /**
   * If this call fails CK is nudged for a human intervention for the future (so the call is NOT passive)
   *
   *  @param vendor_cat_id
   *  @param vendor_cat_string
   *  @param vendor
   *  @return - the data from the api
  */
  public String GETPartnersyndicateRequestcat( String vendor_cat_id, String vendor_cat_string, String vendor) {
    Hashtable p = new Hashtable();
    p.Add("vendor_cat_id",vendor_cat_id);
    p.Add("vendor_cat_string",vendor_cat_string);
    p.Add("vendor",vendor);
    return doCurl("GET","/partnersyndicate/requestcat",p);
  }


  /**
   * This will do nothing if the entity does not have a current partnersyndicate block. Syndication is invoked automatically when entities are saved, so this endpoint is designed for checking the status of syndication.
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String POSTPartnersyndicateUpdateToCk( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("POST","/partnersyndicate/updateToCk",p);
  }


  /**
   * When a plugin is added to the system it must be added to the service
   *
   *  @param id
   *  @param slug
   *  @param owner
   *  @param scope
   *  @param status
   *  @param params
   *  @return - the data from the api
  */
  public String POSTPlugin( String id, String slug, String owner, String scope, String status, String params) {
    Hashtable p = new Hashtable();
    p.Add("id",id);
    p.Add("slug",slug);
    p.Add("owner",owner);
    p.Add("scope",scope);
    p.Add("status",status);
    p.Add("params",params);
    return doCurl("POST","/plugin",p);
  }


  /**
   * Get plugin data
   *
   *  @param id
   *  @return - the data from the api
  */
  public String GETPlugin( String id) {
    Hashtable p = new Hashtable();
    p.Add("id",id);
    return doCurl("GET","/plugin",p);
  }


  /**
   * With a known entity id, a plugin is enabled
   *
   *  @param entity_id
   *  @param plugin
   *  @param inapp_name
   *  @param expiry_date
   *  @return - the data from the api
  */
  public String POSTPluginActivate( String entity_id, String plugin, String inapp_name, String expiry_date) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("plugin",plugin);
    p.Add("inapp_name",inapp_name);
    p.Add("expiry_date",expiry_date);
    return doCurl("POST","/plugin/activate",p);
  }


  /**
   * With a known entity id, a plugin is cancelled
   *
   *  @param entity_id
   *  @param plugin
   *  @param inapp_name
   *  @param expiry_date
   *  @return - the data from the api
  */
  public String POSTPluginCancel( String entity_id, String plugin, String inapp_name, String expiry_date) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("plugin",plugin);
    p.Add("inapp_name",inapp_name);
    p.Add("expiry_date",expiry_date);
    return doCurl("POST","/plugin/cancel",p);
  }


  /**
   * Arbitrary big data
   *
   *  @param pluginid
   *  @param name
   *  @param filter1
   *  @param filter2
   *  @param order
   *  @param fields - a json string with up to 20 fields indexed 'field1' thru 'field20'
   *  @return - the data from the api
  */
  public String GETPluginDatarow( String pluginid, String name, String filter1, String filter2, String order, String fields) {
    Hashtable p = new Hashtable();
    p.Add("pluginid",pluginid);
    p.Add("name",name);
    p.Add("filter1",filter1);
    p.Add("filter2",filter2);
    p.Add("order",order);
    p.Add("fields",fields);
    return doCurl("GET","/plugin/datarow",p);
  }


  /**
   * Arbitrary big data
   *
   *  @param rowdataid
   *  @param pluginid
   *  @param name
   *  @param filter1
   *  @param filter2
   *  @param fields - a json string with up to 20 fields indexed 'field1' thru 'field20'
   *  @return - the data from the api
  */
  public String POSTPluginDatarow( String rowdataid, String pluginid, String name, String filter1, String filter2, String fields) {
    Hashtable p = new Hashtable();
    p.Add("rowdataid",rowdataid);
    p.Add("pluginid",pluginid);
    p.Add("name",name);
    p.Add("filter1",filter1);
    p.Add("filter2",filter2);
    p.Add("fields",fields);
    return doCurl("POST","/plugin/datarow",p);
  }


  /**
   * With a known entity id, a plugin is enabled
   *
   *  @param pluginid
   *  @param userid
   *  @param entity_id
   *  @param storekey
   *  @param storeval
   *  @return - the data from the api
  */
  public String POSTPluginVar( String pluginid, String userid, String entity_id, String storekey, String storeval) {
    Hashtable p = new Hashtable();
    p.Add("pluginid",pluginid);
    p.Add("userid",userid);
    p.Add("entity_id",entity_id);
    p.Add("storekey",storekey);
    p.Add("storeval",storeval);
    return doCurl("POST","/plugin/var",p);
  }


  /**
   * Get variables related to a particular entity
   *
   *  @param entityid
   *  @return - the data from the api
  */
  public String GETPluginVarsByEntityId( String entityid) {
    Hashtable p = new Hashtable();
    p.Add("entityid",entityid);
    return doCurl("GET","/plugin/vars/byEntityId",p);
  }


  /**
   * Get info on all plugins
   *
   *  @return - the data from the api
  */
  public String GETPlugins() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/plugins",p);
  }


  /**
   * Allows a private object to be removed
   *
   *  @param private_object_id - The id of the private object to remove
   *  @return - the data from the api
  */
  public String DELETEPrivate_object( String private_object_id) {
    Hashtable p = new Hashtable();
    p.Add("private_object_id",private_object_id);
    return doCurl("DELETE","/private_object",p);
  }


  /**
   * With a known entity id, a private object can be added.
   *
   *  @param entity_id - The entity to associate the private object with
   *  @param data - The data to store
   *  @return - the data from the api
  */
  public String PUTPrivate_object( String entity_id, String data) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("data",data);
    return doCurl("PUT","/private_object",p);
  }


  /**
   * Allows a private object to be returned based on the entity_id and masheryid
   *
   *  @param entity_id - The entity associated with the private object
   *  @return - the data from the api
  */
  public String GETPrivate_objectAll( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/private_object/all",p);
  }


  /**
   * Update/Add a product
   *
   *  @param product_id - The ID of the product
   *  @param shortname - Desc
   *  @param name - The name of the product
   *  @param strapline - The description of the product
   *  @param alternate_title - The alternate title of the product
   *  @param fpzones - Hints for flatpack display (set a single hint 'void' to have this ignored)
   *  @param paygateid - The product id in the payment gateway (required for Stripe)
   *  @param price - The price of the product
   *  @param tax_rate - The tax markup for this product
   *  @param currency - The currency in which the price is in
   *  @param active - is this an active product
   *  @param billing_period
   *  @param title - Title of the product
   *  @param intro_paragraph - introduction paragraph
   *  @param bullets - pipe separated product features
   *  @param outro_paragraph - closing paragraph
   *  @param product_description_html - Overriding product description html blob
   *  @param thankyou_html - overriding thank you paragraph html
   *  @param thanks_paragraph - thank you paragraph
   *  @param video_url - video url
   *  @return - the data from the api
  */
  public String POSTProduct( String product_id, String shortname, String name, String strapline, String alternate_title, String fpzones, String paygateid, String price, String tax_rate, String currency, String active, String billing_period, String title, String intro_paragraph, String bullets, String outro_paragraph, String product_description_html, String thankyou_html, String thanks_paragraph, String video_url) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("shortname",shortname);
    p.Add("name",name);
    p.Add("strapline",strapline);
    p.Add("alternate_title",alternate_title);
    p.Add("fpzones",fpzones);
    p.Add("paygateid",paygateid);
    p.Add("price",price);
    p.Add("tax_rate",tax_rate);
    p.Add("currency",currency);
    p.Add("active",active);
    p.Add("billing_period",billing_period);
    p.Add("title",title);
    p.Add("intro_paragraph",intro_paragraph);
    p.Add("bullets",bullets);
    p.Add("outro_paragraph",outro_paragraph);
    p.Add("product_description_html",product_description_html);
    p.Add("thankyou_html",thankyou_html);
    p.Add("thanks_paragraph",thanks_paragraph);
    p.Add("video_url",video_url);
    return doCurl("POST","/product",p);
  }


  /**
   * Returns the product information given a valid product_id
   *
   *  @param product_id
   *  @return - the data from the api
  */
  public String GETProduct( String product_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    return doCurl("GET","/product",p);
  }


  /**
   * Uploads logo image to product
   *
   *  @param product_id
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTProductImageLogo( String product_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/product/image/logo",p);
  }


  /**
   * Delete the logo image within a specific product
   *
   *  @param product_id
   *  @return - the data from the api
  */
  public String DELETEProductImageLogo( String product_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    return doCurl("DELETE","/product/image/logo",p);
  }


  /**
   * Delete the main image within a specific product
   *
   *  @param product_id
   *  @return - the data from the api
  */
  public String DELETEProductImageMain( String product_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    return doCurl("DELETE","/product/image/main",p);
  }


  /**
   * Adds partblahnersyndicate provisioning object to a product
   *
   *  @param product_id
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTProductImageMain( String product_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/product/image/main",p);
  }


  /**
   * Delete the small image within a specific product
   *
   *  @param product_id
   *  @return - the data from the api
  */
  public String DELETEProductImageSmall( String product_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    return doCurl("DELETE","/product/image/small",p);
  }


  /**
   * Uploads small image to product
   *
   *  @param product_id
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTProductImageSmall( String product_id, String filedata) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("filedata",filedata);
    return doCurl("POST","/product/image/small",p);
  }


  /**
   * Removes a provisioning object from product
   *
   *  @param product_id
   *  @param gen_id
   *  @return - the data from the api
  */
  public String DELETEProductProvisioning( String product_id, String gen_id) {
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
  public String POSTProductProvisioningAdvert( String product_id, String publisher_id, String max_tags, String max_locations) {
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
   *  @param package
   *  @return - the data from the api
  */
  public String POSTProductProvisioningClaim( String product_id, String package) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("package",package);
    return doCurl("POST","/product/provisioning/claim",p);
  }


  /**
   * Adds maxclaims provisioning object to a product
   *
   *  @param product_id
   *  @param country
   *  @param number
   *  @return - the data from the api
  */
  public String POSTProductProvisioningMaxclaims( String product_id, String country, String number) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("country",country);
    p.Add("number",number);
    return doCurl("POST","/product/provisioning/maxclaims",p);
  }


  /**
   * Adds partnersyndicate provisioning object to a product
   *
   *  @param product_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String POSTProductProvisioningPartnersyndicate( String product_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("POST","/product/provisioning/partnersyndicate",p);
  }


  /**
   * Adds plugin provisioning object to a product
   *
   *  @param product_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String POSTProductProvisioningPlugin( String product_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("POST","/product/provisioning/plugin",p);
  }


  /**
   * Adds SCheduleSMS provisioning object to a product
   *
   *  @param product_id
   *  @param package
   *  @return - the data from the api
  */
  public String POSTProductProvisioningSchedulesms( String product_id, String package) {
    Hashtable p = new Hashtable();
    p.Add("product_id",product_id);
    p.Add("package",package);
    return doCurl("POST","/product/provisioning/schedulesms",p);
  }


  /**
   * Adds syndication provisioning object to a product
   *
   *  @param product_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String POSTProductProvisioningSyndication( String product_id, String publisher_id) {
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
  public String GETPtbAll( String entity_id, String destructive) {
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
  public String GETPtbLog( String year, String month, String entity_id) {
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
  public String GETPtbModule( String entity_id, String module, String destructive) {
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
  public String GETPtbRunrate( String country, String year, String month, String day) {
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
  public String GETPtbValueadded( String country, String year, String month, String day) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    p.Add("year",year);
    p.Add("month",month);
    p.Add("day",day);
    return doCurl("GET","/ptb/valueadded",p);
  }


  /**
   * Returns publisher that matches a given publisher id
   *
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String GETPublisher( String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("publisher_id",publisher_id);
    return doCurl("GET","/publisher",p);
  }


  /**
   * Update/Add a publisher
   *
   *  @param publisher_id
   *  @param country
   *  @param name
   *  @param description
   *  @param active
   *  @param url_patterns
   *  @param premium_adverts_platinum
   *  @param premium_adverts_gold
   *  @return - the data from the api
  */
  public String POSTPublisher( String publisher_id, String country, String name, String description, String active, String url_patterns, String premium_adverts_platinum, String premium_adverts_gold) {
    Hashtable p = new Hashtable();
    p.Add("publisher_id",publisher_id);
    p.Add("country",country);
    p.Add("name",name);
    p.Add("description",description);
    p.Add("active",active);
    p.Add("url_patterns",url_patterns);
    p.Add("premium_adverts_platinum",premium_adverts_platinum);
    p.Add("premium_adverts_gold",premium_adverts_gold);
    return doCurl("POST","/publisher",p);
  }


  /**
   * Delete a publisher with a specified publisher_id
   *
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String DELETEPublisher( String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("publisher_id",publisher_id);
    return doCurl("DELETE","/publisher",p);
  }


  /**
   * Returns publisher that matches a given publisher id
   *
   *  @param country
   *  @return - the data from the api
  */
  public String GETPublisherByCountry( String country) {
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
  public String GETPublisherByEntityId( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/publisher/byEntityId",p);
  }


  /**
   * Returns a publisher that has the specified masheryid
   *
   *  @param publisher_masheryid
   *  @return - the data from the api
  */
  public String GETPublisherBy_masheryid( String publisher_masheryid) {
    Hashtable p = new Hashtable();
    p.Add("publisher_masheryid",publisher_masheryid);
    return doCurl("GET","/publisher/by_masheryid",p);
  }


  /**
   * Retrieve queue items.
   *
   *  @param limit
   *  @param queue_name
   *  @return - the data from the api
  */
  public String GETQueue( String limit, String queue_name) {
    Hashtable p = new Hashtable();
    p.Add("limit",limit);
    p.Add("queue_name",queue_name);
    return doCurl("GET","/queue",p);
  }


  /**
   * Create a queue item
   *
   *  @param queue_name
   *  @param data
   *  @return - the data from the api
  */
  public String PUTQueue( String queue_name, String data) {
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
  public String DELETEQueue( String queue_id) {
    Hashtable p = new Hashtable();
    p.Add("queue_id",queue_id);
    return doCurl("DELETE","/queue",p);
  }


  /**
   * Find a queue item by its cloudant id
   *
   *  @param queue_id
   *  @return - the data from the api
  */
  public String GETQueueBy_id( String queue_id) {
    Hashtable p = new Hashtable();
    p.Add("queue_id",queue_id);
    return doCurl("GET","/queue/by_id",p);
  }


  /**
   * Add an error to a queue item
   *
   *  @param queue_id
   *  @param error
   *  @return - the data from the api
  */
  public String POSTQueueError( String queue_id, String error) {
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
  public String GETQueueSearch( String type, String id) {
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
  public String POSTQueueUnlock( String queue_name, String seconds) {
    Hashtable p = new Hashtable();
    p.Add("queue_name",queue_name);
    p.Add("seconds",seconds);
    return doCurl("POST","/queue/unlock",p);
  }


  /**
   * Create an SQS queue item
   *
   *  @param queue_name
   *  @param data
   *  @return - the data from the api
  */
  public String PUTQueue_sqs( String queue_name, String data) {
    Hashtable p = new Hashtable();
    p.Add("queue_name",queue_name);
    p.Add("data",data);
    return doCurl("PUT","/queue_sqs",p);
  }


  /**
   * Get the attributes of an SQS queue
   *
   *  @param queue_name
   *  @return - the data from the api
  */
  public String GETQueue_sqsAttributes( String queue_name) {
    Hashtable p = new Hashtable();
    p.Add("queue_name",queue_name);
    return doCurl("GET","/queue_sqs/attributes",p);
  }


  /**
   * Returns reseller that matches a given reseller id
   *
   *  @param reseller_id
   *  @return - the data from the api
  */
  public String GETReseller( String reseller_id) {
    Hashtable p = new Hashtable();
    p.Add("reseller_id",reseller_id);
    return doCurl("GET","/reseller",p);
  }


  /**
   * Update/Add a reseller
   *
   *  @param reseller_id
   *  @param country
   *  @param name
   *  @param description
   *  @param active
   *  @param products
   *  @param master_user_id
   *  @param canViewEmployee
   *  @return - the data from the api
  */
  public String POSTReseller( String reseller_id, String country, String name, String description, String active, String products, String master_user_id, String canViewEmployee) {
    Hashtable p = new Hashtable();
    p.Add("reseller_id",reseller_id);
    p.Add("country",country);
    p.Add("name",name);
    p.Add("description",description);
    p.Add("active",active);
    p.Add("products",products);
    p.Add("master_user_id",master_user_id);
    p.Add("canViewEmployee",canViewEmployee);
    return doCurl("POST","/reseller",p);
  }


  /**
   * Return a sales log by id
   *
   *  @param sales_log_id - The sales log id to pull
   *  @return - the data from the api
  */
  public String GETSales_log( String sales_log_id) {
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
  public String GETSales_logBy_countryBy_date( String from_date, String country, String action_type) {
    Hashtable p = new Hashtable();
    p.Add("from_date",from_date);
    p.Add("country",country);
    p.Add("action_type",action_type);
    return doCurl("GET","/sales_log/by_country/by_date",p);
  }


  /**
   * Return a sales log by date range, filtered by masheryid if it is given
   *
   *  @param from_date
   *  @param to_date
   *  @param group
   *  @param limit - Applicable only when group=false
   *  @param skip - Applicable only when group=false
   *  @return - the data from the api
  */
  public String GETSales_logBy_date( String from_date, String to_date, String group, String limit, String skip) {
    Hashtable p = new Hashtable();
    p.Add("from_date",from_date);
    p.Add("to_date",to_date);
    p.Add("group",group);
    p.Add("limit",limit);
    p.Add("skip",skip);
    return doCurl("GET","/sales_log/by_date",p);
  }


  /**
   * Log a sale
   *
   *  @param entity_id - The entity the sale was made against
   *  @param country - The country code the sales log orginated
   *  @param action_type - The type of action we are performing
   *  @param ad_type - The type of advertisements
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
  public String POSTSales_logEntity( String entity_id, String country, String action_type, String ad_type, String publisher_id, String mashery_id, String reseller_ref, String reseller_agent_id, String max_tags, String max_locations, String extra_tags, String extra_locations, String expiry_date) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("country",country);
    p.Add("action_type",action_type);
    p.Add("ad_type",ad_type);
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
  public String POSTSales_logSyndication( String action_type, String syndication_type, String publisher_id, String expiry_date, String entity_id, String group_id, String seed_masheryid, String supplier_masheryid, String country, String reseller_masheryid) {
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
   * Converts an Entity into a submission at the Scoot Partner API
   *
   *  @param entity_id - The entity to parse
   *  @param reseller - The reseller Mashery ID, it also determines which Scoot API key to use
   *  @param scoot_id - If specified, the related Scoot listing will be updated.
   *  @param autofill_scoot_id - If scoot_id is not given, look for past successful syndication logs to fill in the Scoot ID
   *  @return - the data from the api
  */
  public String POSTScoot_priority( String entity_id, String reseller, String scoot_id, String autofill_scoot_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("reseller",reseller);
    p.Add("scoot_id",scoot_id);
    p.Add("autofill_scoot_id",autofill_scoot_id);
    return doCurl("POST","/scoot_priority",p);
  }


  /**
   * Make a url shorter
   *
   *  @param url - the url to shorten
   *  @param idOnly - Return just the Shortened URL ID
   *  @return - the data from the api
  */
  public String PUTShortenurl( String url, String idOnly) {
    Hashtable p = new Hashtable();
    p.Add("url",url);
    p.Add("idOnly",idOnly);
    return doCurl("PUT","/shortenurl",p);
  }


  /**
   * get the long url from the db
   *
   *  @param id - the id to fetch from the db
   *  @return - the data from the api
  */
  public String GETShortenurl( String id) {
    Hashtable p = new Hashtable();
    p.Add("id",id);
    return doCurl("GET","/shortenurl",p);
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
   *  @param feedback - Some feedback from the person submitting the signal
   *  @return - the data from the api
  */
  public String POSTSignal( String entity_id, String country, String gen_id, String signal_type, String data_type, String inactive_reason, String inactive_description, String feedback) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("country",country);
    p.Add("gen_id",gen_id);
    p.Add("signal_type",signal_type);
    p.Add("data_type",data_type);
    p.Add("inactive_reason",inactive_reason);
    p.Add("inactive_description",inactive_description);
    p.Add("feedback",feedback);
    return doCurl("POST","/signal",p);
  }


  /**
   * With a given country and entity id suffix, this endpoint will return a list of entity IDs and their last modified dates for sitemap generation.
   *
   *  @param country - Target country code.
   *  @param id_suffix - Target entity Id suffix (4 digits).
   *  @param skip
   *  @param limit
   *  @return - the data from the api
  */
  public String GETSitemapEntity( String country, String id_suffix, String skip, String limit) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    p.Add("id_suffix",id_suffix);
    p.Add("skip",skip);
    p.Add("limit",limit);
    return doCurl("GET","/sitemap/entity",p);
  }


  /**
   * With a given country, this endpoint will return a list of entity ID suffixes which have records.
   *
   *  @param country - Target country code.
   *  @return - the data from the api
  */
  public String GETSitemapEntitySummary( String country) {
    Hashtable p = new Hashtable();
    p.Add("country",country);
    return doCurl("GET","/sitemap/entity/summary",p);
  }


  /**
   * Get a spider document
   *
   *  @param spider_id
   *  @return - the data from the api
  */
  public String GETSpider( String spider_id) {
    Hashtable p = new Hashtable();
    p.Add("spider_id",spider_id);
    return doCurl("GET","/spider",p);
  }


  /**
   * Get the number of times an entity has been served out as an advert or on serps/bdp pages
   *
   *  @param entity_id - A valid entity_id e.g. 379236608286720
   *  @param year - The year to report on
   *  @param month - The month to report on
   *  @return - the data from the api
  */
  public String GETStatsEntityBy_date( String entity_id, String year, String month) {
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
  public String GETStatsEntityBy_year( String entity_id, String year) {
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
  public String GETStatus() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/status",p);
  }


  /**
   * get a Syndication
   *
   *  @param syndication_id
   *  @return - the data from the api
  */
  public String GETSyndication( String syndication_id) {
    Hashtable p = new Hashtable();
    p.Add("syndication_id",syndication_id);
    return doCurl("GET","/syndication",p);
  }


  /**
   * get a Syndication by entity_id
   *
   *  @param entity_id
   *  @return - the data from the api
  */
  public String GETSyndicationBy_entity_id( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/syndication/by_entity_id",p);
  }


  /**
   * Get a Syndication by Reseller (Mashery ID) and optional entity ID
   *
   *  @param reseller_masheryid
   *  @param entity_id
   *  @return - the data from the api
  */
  public String GETSyndicationBy_reseller( String reseller_masheryid, String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("reseller_masheryid",reseller_masheryid);
    p.Add("entity_id",entity_id);
    return doCurl("GET","/syndication/by_reseller",p);
  }


  /**
   * Cancel a syndication
   *
   *  @param syndication_id
   *  @return - the data from the api
  */
  public String POSTSyndicationCancel( String syndication_id) {
    Hashtable p = new Hashtable();
    p.Add("syndication_id",syndication_id);
    return doCurl("POST","/syndication/cancel",p);
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
   *  @param data_filter
   *  @return - the data from the api
  */
  public String POSTSyndicationCreate( String syndication_type, String publisher_id, String expiry_date, String entity_id, String group_id, String seed_masheryid, String supplier_masheryid, String country, String data_filter) {
    Hashtable p = new Hashtable();
    p.Add("syndication_type",syndication_type);
    p.Add("publisher_id",publisher_id);
    p.Add("expiry_date",expiry_date);
    p.Add("entity_id",entity_id);
    p.Add("group_id",group_id);
    p.Add("seed_masheryid",seed_masheryid);
    p.Add("supplier_masheryid",supplier_masheryid);
    p.Add("country",country);
    p.Add("data_filter",data_filter);
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
   *  @param expiry_date
   *  @return - the data from the api
  */
  public String POSTSyndicationRenew( String syndication_type, String publisher_id, String entity_id, String group_id, String seed_masheryid, String supplier_masheryid, String country, String expiry_date) {
    Hashtable p = new Hashtable();
    p.Add("syndication_type",syndication_type);
    p.Add("publisher_id",publisher_id);
    p.Add("entity_id",entity_id);
    p.Add("group_id",group_id);
    p.Add("seed_masheryid",seed_masheryid);
    p.Add("supplier_masheryid",supplier_masheryid);
    p.Add("country",country);
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
   *  @param reseller_id - The optional reseller id used in the syndications
   *  @return - the data from the api
  */
  public String POSTSyndication_log( String entity_id, String publisher_id, String action, String success, String message, String syndicated_id, String reseller_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("publisher_id",publisher_id);
    p.Add("action",action);
    p.Add("success",success);
    p.Add("message",message);
    p.Add("syndicated_id",syndicated_id);
    p.Add("reseller_id",reseller_id);
    return doCurl("POST","/syndication_log",p);
  }


  /**
   * Get all syndication log entries for a given entity id
   *
   *  @param entity_id
   *  @param page
   *  @param per_page
   *  @return - the data from the api
  */
  public String GETSyndication_logBy_entity_id( String entity_id, String page, String per_page) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("page",page);
    p.Add("per_page",per_page);
    return doCurl("GET","/syndication_log/by_entity_id",p);
  }


  /**
   * Get the latest syndication log feedback entry for a given entity id and publisher
   *
   *  @param entity_id
   *  @param publisher_id
   *  @return - the data from the api
  */
  public String GETSyndication_logLast_syndicated_id( String entity_id, String publisher_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("publisher_id",publisher_id);
    return doCurl("GET","/syndication_log/last_syndicated_id",p);
  }


  /**
   * Creates a new Syndication Submission
   *
   *  @param syndication_type
   *  @param entity_id
   *  @param publisher_id
   *  @param submission_id
   *  @return - the data from the api
  */
  public String PUTSyndication_submission( String syndication_type, String entity_id, String publisher_id, String submission_id) {
    Hashtable p = new Hashtable();
    p.Add("syndication_type",syndication_type);
    p.Add("entity_id",entity_id);
    p.Add("publisher_id",publisher_id);
    p.Add("submission_id",submission_id);
    return doCurl("PUT","/syndication_submission",p);
  }


  /**
   * Returns a Syndication Submission
   *
   *  @param syndication_submission_id
   *  @return - the data from the api
  */
  public String GETSyndication_submission( String syndication_submission_id) {
    Hashtable p = new Hashtable();
    p.Add("syndication_submission_id",syndication_submission_id);
    return doCurl("GET","/syndication_submission",p);
  }


  /**
   * Set active to false for a Syndication Submission
   *
   *  @param syndication_submission_id
   *  @return - the data from the api
  */
  public String POSTSyndication_submissionDeactivate( String syndication_submission_id) {
    Hashtable p = new Hashtable();
    p.Add("syndication_submission_id",syndication_submission_id);
    return doCurl("POST","/syndication_submission/deactivate",p);
  }


  /**
   * Set the processed date to now for a Syndication Submission
   *
   *  @param syndication_submission_id
   *  @return - the data from the api
  */
  public String POSTSyndication_submissionProcessed( String syndication_submission_id) {
    Hashtable p = new Hashtable();
    p.Add("syndication_submission_id",syndication_submission_id);
    return doCurl("POST","/syndication_submission/processed",p);
  }


  /**
   * Provides a tokenised URL to redirect a user so they can add an entity to Central Index
   *
   *  @param language - The language to use to render the add path e.g. en
   *  @param business_name - The name of the business (to be presented as a default) e.g. The Dog and Duck
   *  @param business_phone - The phone number of the business (to be presented as a default) e.g. 20 8480-2777
   *  @param business_postcode - The postcode of the business (to be presented as a default) e.g. EC1 1AA
   *  @param portal_name - The name of the website that data is to be added on e.g. YourLocal
   *  @param supplier_id - The supplier id e.g. 1234xxx889
   *  @param partner - syndication partner (eg 192)
   *  @param country - The country of the entity to be added e.g. gb
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @return - the data from the api
  */
  public String GETTokenAdd( String language, String business_name, String business_phone, String business_postcode, String portal_name, String supplier_id, String partner, String country, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("language",language);
    p.Add("business_name",business_name);
    p.Add("business_phone",business_phone);
    p.Add("business_postcode",business_postcode);
    p.Add("portal_name",portal_name);
    p.Add("supplier_id",supplier_id);
    p.Add("partner",partner);
    p.Add("country",country);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/add",p);
  }


  /**
   * Provides a tokenised URL to redirect a user to claim an entity on Central Index
   *
   *  @param entity_id - Entity ID to be claimed e.g. 380348266819584
   *  @param supplier_id - Supplier ID to be added (along with masheryid) e.g. 380348266819584
   *  @param language - The language to use to render the claim path e.g. en
   *  @param portal_name - The name of the website that entity is being claimed on e.g. YourLocal
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param admin_host - The admin host to refer back to - will only be respected if whitelisted in configuration
   *  @return - the data from the api
  */
  public String GETTokenClaim( String entity_id, String supplier_id, String language, String portal_name, String flatpack_id, String admin_host) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("supplier_id",supplier_id);
    p.Add("language",language);
    p.Add("portal_name",portal_name);
    p.Add("flatpack_id",flatpack_id);
    p.Add("admin_host",admin_host);
    return doCurl("GET","/token/claim",p);
  }


  /**
   * Fetch token for the contact us method
   *
   *  @param portal_name - The portal name
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param language - en, es, etc...
   *  @param referring_url - the url where the request came from
   *  @return - the data from the api
  */
  public String GETTokenContact_us( String portal_name, String flatpack_id, String language, String referring_url) {
    Hashtable p = new Hashtable();
    p.Add("portal_name",portal_name);
    p.Add("flatpack_id",flatpack_id);
    p.Add("language",language);
    p.Add("referring_url",referring_url);
    return doCurl("GET","/token/contact_us",p);
  }


  /**
   * Allows us to identify the user, entity and element from an encoded endpoint URL's token
   *
   *  @param token
   *  @return - the data from the api
  */
  public String GETTokenDecode( String token) {
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
  public String GETTokenEdit( String entity_id, String language, String flatpack_id, String edit_page) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    p.Add("edit_page",edit_page);
    return doCurl("GET","/token/edit",p);
  }


  /**
   * Fetch token for some admin page.
   *
   *  @param portal_name - The name of the application that has initiated the login process, example: 'Your Local'
   *  @param code - Secret string which will be validated by the target page.
   *  @param expireAt - When this token expires in UNIX timestamp. The target page should validate this.
   *  @param language - The language for the app
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param multipack_id - The id of the multipack site where the request originated
   *  @param data - Optional extra data to be passed to the targeted page.
   *  @return - the data from the api
  */
  public String GETTokenEncode( String portal_name, String code, String expireAt, String language, String flatpack_id, String multipack_id, String data) {
    Hashtable p = new Hashtable();
    p.Add("portal_name",portal_name);
    p.Add("code",code);
    p.Add("expireAt",expireAt);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    p.Add("multipack_id",multipack_id);
    p.Add("data",data);
    return doCurl("GET","/token/encode",p);
  }


  /**
   * Fetch token for login path
   *
   *  @param portal_name - The name of the application that has initiated the login process, example: 'Your Local'
   *  @param language - The language for the app
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param multipack_id - The id of the multipack site where the request originated
   *  @return - the data from the api
  */
  public String GETTokenLogin( String portal_name, String language, String flatpack_id, String multipack_id) {
    Hashtable p = new Hashtable();
    p.Add("portal_name",portal_name);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    p.Add("multipack_id",multipack_id);
    return doCurl("GET","/token/login",p);
  }


  /**
   * Get a tokenised url for an password reset
   *
   *  @param login_id - Login id
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param entity_id
   *  @param action
   *  @return - the data from the api
  */
  public String GETTokenLoginReset_password( String login_id, String flatpack_id, String entity_id, String action) {
    Hashtable p = new Hashtable();
    p.Add("login_id",login_id);
    p.Add("flatpack_id",flatpack_id);
    p.Add("entity_id",entity_id);
    p.Add("action",action);
    return doCurl("GET","/token/login/reset_password",p);
  }


  /**
   * Get a tokenised url for an email verification
   *
   *  @param email - Email address
   *  @param first_name - First name of the new user
   *  @param last_name - Last name of the new user
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param entity_id
   *  @param action
   *  @return - the data from the api
  */
  public String GETTokenLoginSet_password( String email, String first_name, String last_name, String flatpack_id, String entity_id, String action) {
    Hashtable p = new Hashtable();
    p.Add("email",email);
    p.Add("first_name",first_name);
    p.Add("last_name",last_name);
    p.Add("flatpack_id",flatpack_id);
    p.Add("entity_id",entity_id);
    p.Add("action",action);
    return doCurl("GET","/token/login/set_password",p);
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
  public String GETTokenMessage( String entity_id, String portal_name, String language, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("portal_name",portal_name);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/message",p);
  }


  /**
   * Fetch token for partnerclaim path
   *
   *  @param language - The language for the app
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param partner - The partner (eg 192)
   *  @param partnerid - the supplier id from the partner site
   *  @param preclaimed - is this already claimed on the partner site (used for messaging)
   *  @return - the data from the api
  */
  public String GETTokenPartnerclaim( String language, String flatpack_id, String partner, String partnerid, String preclaimed) {
    Hashtable p = new Hashtable();
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    p.Add("partner",partner);
    p.Add("partnerid",partnerid);
    p.Add("preclaimed",preclaimed);
    return doCurl("GET","/token/partnerclaim",p);
  }


  /**
   * Fetch token for partnerclaim path (ie we start at a CI entity id rather than an external partner id)
   *
   *  @param language - The language for the app
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param partner - The partner (eg 192)
   *  @param entityid - the CI entity id
   *  @param preclaimed - is this already claimed on the partner site (used for messaging)
   *  @return - the data from the api
  */
  public String GETTokenPartnerclaimInternal( String language, String flatpack_id, String partner, String entityid, String preclaimed) {
    Hashtable p = new Hashtable();
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    p.Add("partner",partner);
    p.Add("entityid",entityid);
    p.Add("preclaimed",preclaimed);
    return doCurl("GET","/token/partnerclaim/internal",p);
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
  public String GETTokenProduct( String entity_id, String product_id, String language, String portal_name, String flatpack_id, String source, String channel, String campaign) {
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
   * Fetch token for product path
   *
   *  @param entity_id - The id of the entity to add a product to
   *  @param portal_name - The portal name
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param language - en, es, etc...
   *  @return - the data from the api
  */
  public String GETTokenProduct_selector( String entity_id, String portal_name, String flatpack_id, String language) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("portal_name",portal_name);
    p.Add("flatpack_id",flatpack_id);
    p.Add("language",language);
    return doCurl("GET","/token/product_selector",p);
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
  public String GETTokenReport( String entity_id, String portal_name, String language, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("portal_name",portal_name);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/report",p);
  }


  /**
   * Get a tokenised url for the review path
   *
   *  @param portal_name - The portal name
   *  @param entity_id
   *  @param language - en, es, etc...
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @return - the data from the api
  */
  public String GETTokenReview( String portal_name, String entity_id, String language, String flatpack_id) {
    Hashtable p = new Hashtable();
    p.Add("portal_name",portal_name);
    p.Add("entity_id",entity_id);
    p.Add("language",language);
    p.Add("flatpack_id",flatpack_id);
    return doCurl("GET","/token/review",p);
  }


  /**
   * Get a tokenised url for the testimonial path
   *
   *  @param portal_name - The portal name
   *  @param flatpack_id - The id of the flatpack site where the request originated
   *  @param language - en, es, etc...
   *  @param entity_id
   *  @param shorten_url
   *  @return - the data from the api
  */
  public String GETTokenTestimonial( String portal_name, String flatpack_id, String language, String entity_id, String shorten_url) {
    Hashtable p = new Hashtable();
    p.Add("portal_name",portal_name);
    p.Add("flatpack_id",flatpack_id);
    p.Add("language",language);
    p.Add("entity_id",entity_id);
    p.Add("shorten_url",shorten_url);
    return doCurl("GET","/token/testimonial",p);
  }


  /**
   * The JaroWinklerDistance between two entities postal address objects
   *
   *  @param first_entity_id - The entity id of the first entity to be used in the postal address difference
   *  @param second_entity_id - The entity id of the second entity to be used in the postal address difference
   *  @return - the data from the api
  */
  public String GETToolsAddressdiff( String first_entity_id, String second_entity_id) {
    Hashtable p = new Hashtable();
    p.Add("first_entity_id",first_entity_id);
    p.Add("second_entity_id",second_entity_id);
    return doCurl("GET","/tools/addressdiff",p);
  }


  /**
   * An API call to test crashing the server
   *
   *  @return - the data from the api
  */
  public String GETToolsCrash() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/tools/crash",p);
  }


  /**
   * Provide a method, a path and some data to run a load of curl commands and get emailed when complete
   *
   *  @param method - The method e.g. POST
   *  @param path - The relative api call e.g. /entity/phone
   *  @param filedata - A tab separated file for ingest
   *  @param email - Response email address e.g. dave@fender.com
   *  @return - the data from the api
  */
  public String POSTToolsCurl( String method, String path, String filedata, String email) {
    Hashtable p = new Hashtable();
    p.Add("method",method);
    p.Add("path",path);
    p.Add("filedata",filedata);
    p.Add("email",email);
    return doCurl("POST","/tools/curl",p);
  }


  /**
   * Use this call to get information (in HTML or JSON) about the data structure of given entity object (e.g. a phone number or an address)
   *
   *  @param object - The API call documentation is required for
   *  @param format - The format of the returned data eg. JSON or HTML
   *  @return - the data from the api
  */
  public String GETToolsDocs( String _object, String format) {
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
  public String GETToolsFormatAddress( String address, String country) {
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
   *  @param ignoreRegionCheck - If ture, we only check if the phone number is valid, ignoring country context
   *  @return - the data from the api
  */
  public String GETToolsFormatPhone( String number, String country, String ignoreRegionCheck) {
    Hashtable p = new Hashtable();
    p.Add("number",number);
    p.Add("country",country);
    p.Add("ignoreRegionCheck",ignoreRegionCheck);
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
  public String GETToolsGeocode( String building_number, String address1, String address2, String address3, String district, String town, String county, String province, String postcode, String country) {
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
   * Given a spreadsheet ID, and a worksheet ID, add a row
   *
   *  @param spreadsheet_key - The key of the spreadsheet to edit
   *  @param worksheet_key - The key of the worksheet to edit - failure to provide this will assume worksheet with the label 'Sheet1'
   *  @param data - A comma separated list to add as cells
   *  @return - the data from the api
  */
  public String POSTToolsGooglesheetAdd_row( String spreadsheet_key, String worksheet_key, String data) {
    Hashtable p = new Hashtable();
    p.Add("spreadsheet_key",spreadsheet_key);
    p.Add("worksheet_key",worksheet_key);
    p.Add("data",data);
    return doCurl("POST","/tools/googlesheet/add_row",p);
  }


  /**
   * Given a spreadsheet ID and the name of a worksheet identify the Google ID for the worksheet
   *
   *  @param spreadsheet_key - The key of the spreadsheet
   *  @param worksheet_name - The name/label of the worksheet to identify
   *  @return - the data from the api
  */
  public String POSTToolsGooglesheetWorksheet_id( String spreadsheet_key, String worksheet_name) {
    Hashtable p = new Hashtable();
    p.Add("spreadsheet_key",spreadsheet_key);
    p.Add("worksheet_name",worksheet_name);
    return doCurl("POST","/tools/googlesheet/worksheet_id",p);
  }


  /**
   * Given some image data we can resize and upload the images
   *
   *  @param filedata - The image data to upload and resize
   *  @param type - The type of image to upload and resize
   *  @param image_dir - Set the destination directory of the generated images on S3. Only available when Image API is enabled.
   *  @return - the data from the api
  */
  public String POSTToolsImage( String filedata, String type, String image_dir) {
    Hashtable p = new Hashtable();
    p.Add("filedata",filedata);
    p.Add("type",type);
    p.Add("image_dir",image_dir);
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
  public String GETToolsIodocs( String mode, String path, String endpoint, String doctype) {
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
  public String GETToolsLess( String less) {
    Hashtable p = new Hashtable();
    p.Add("less",less);
    return doCurl("GET","/tools/less",p);
  }


  /**
   * Parse unstructured address data to fit our structured address objects
   *
   *  @param address
   *  @param postcode
   *  @param country
   *  @param normalise
   *  @return - the data from the api
  */
  public String GETToolsParseAddress( String address, String postcode, String country, String normalise) {
    Hashtable p = new Hashtable();
    p.Add("address",address);
    p.Add("postcode",postcode);
    p.Add("country",country);
    p.Add("normalise",normalise);
    return doCurl("GET","/tools/parse/address",p);
  }


  /**
   * Ring the person and verify their account
   *
   *  @param to - The phone number to verify
   *  @param from - The phone number to call from
   *  @param pin - The pin to verify the phone number with
   *  @param twilio_voice - The language to read the verification in
   *  @param extension - The pin to verify the phone number with
   *  @return - the data from the api
  */
  public String GETToolsPhonecallVerify( String to, String from, String pin, String twilio_voice, String extension) {
    Hashtable p = new Hashtable();
    p.Add("to",to);
    p.Add("from",from);
    p.Add("pin",pin);
    p.Add("twilio_voice",twilio_voice);
    p.Add("extension",extension);
    return doCurl("GET","/tools/phonecall/verify",p);
  }


  /**
   * Return the phonetic representation of a string
   *
   *  @param text
   *  @return - the data from the api
  */
  public String GETToolsPhonetic( String text) {
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
  public String GETToolsProcess_phone( String number) {
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
  public String GETToolsProcess_string( String text) {
    Hashtable p = new Hashtable();
    p.Add("text",text);
    return doCurl("GET","/tools/process_string",p);
  }


  /**
   * Force refresh of search indexes
   *
   *  @return - the data from the api
  */
  public String GETToolsReindex() {
    Hashtable p = new Hashtable();
    return doCurl("GET","/tools/reindex",p);
  }


  /**
   * Check to see if a supplied email address is valid
   *
   *  @param from - The phone number from which the SMS orginates
   *  @param to - The phone number to which the SMS is to be sent
   *  @param message - The message to be sent in the SMS
   *  @return - the data from the api
  */
  public String GETToolsSendsms( String from, String to, String message) {
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
   *  @param save
   *  @return - the data from the api
  */
  public String GETToolsSpider( String url, String pages, String country, String save) {
    Hashtable p = new Hashtable();
    p.Add("url",url);
    p.Add("pages",pages);
    p.Add("country",country);
    p.Add("save",save);
    return doCurl("GET","/tools/spider",p);
  }


  /**
   * Returns a stemmed version of a string
   *
   *  @param text
   *  @return - the data from the api
  */
  public String GETToolsStem( String text) {
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
  public String GETToolsStopwords( String text) {
    Hashtable p = new Hashtable();
    p.Add("text",text);
    return doCurl("GET","/tools/stopwords",p);
  }


  /**
   * Fetch the result of submitted data we sent to InfoGroup
   *
   *  @param syndication_submission_id - The syndication_submission_id to fetch info for
   *  @return - the data from the api
  */
  public String GETToolsSubmissionInfogroup( String syndication_submission_id) {
    Hashtable p = new Hashtable();
    p.Add("syndication_submission_id",syndication_submission_id);
    return doCurl("GET","/tools/submission/infogroup",p);
  }


  /**
   * Fetch the entity and convert it to 118 Places CSV format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicate118( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/118",p);
  }


  /**
   * Fetch the entity and convert it to Bing Ads CSV format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateBingads( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/bingads",p);
  }


  /**
   * Fetch the entity and convert it to Bing Places CSV format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateBingplaces( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/bingplaces",p);
  }


  /**
   * Fetch the entity and convert it to DnB TSV format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateDnb( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/dnb",p);
  }


  /**
   * Fetch the entity and convert add it to arlington
   *
   *  @param entity_id - The entity_id to fetch
   *  @param reseller_masheryid - The reseller_masheryid
   *  @param destructive - Add the entity or simulate adding the entity
   *  @param data_filter - The level of filtering to apply to the entity
   *  @return - the data from the api
  */
  public String GETToolsSyndicateEnablemedia( String entity_id, String reseller_masheryid, String destructive, String data_filter) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("reseller_masheryid",reseller_masheryid);
    p.Add("destructive",destructive);
    p.Add("data_filter",data_filter);
    return doCurl("GET","/tools/syndicate/enablemedia",p);
  }


  /**
   * Fetch the entity and convert add it to Factual
   *
   *  @param entity_id - The entity_id to fetch
   *  @param destructive - Add the entity or simulate adding the entity
   *  @return - the data from the api
  */
  public String GETToolsSyndicateFactual( String entity_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("destructive",destructive);
    return doCurl("GET","/tools/syndicate/factual",p);
  }


  /**
   * Fetch the entity and convert it to Factual CSV / TSV format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateFactualcsv( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/factualcsv",p);
  }


  /**
   * Syndicate an entity to Foursquare
   *
   *  @param entity_id - The entity_id to fetch
   *  @param destructive - Add the entity or simulate adding the entity
   *  @return - the data from the api
  */
  public String GETToolsSyndicateFoursquare( String entity_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("destructive",destructive);
    return doCurl("GET","/tools/syndicate/foursquare",p);
  }


  /**
   * Fetch the entity and convert it to TomTom XML format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateGoogle( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/google",p);
  }


  /**
   * Fetch the entity and convert it to Infobel CSV / TSV format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateInfobelcsv( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/infobelcsv",p);
  }


  /**
   * Fetch the entity and convert add it to InfoGroup
   *
   *  @param entity_id - The entity_id to fetch
   *  @param destructive - Add the entity or simulate adding the entity
   *  @return - the data from the api
  */
  public String GETToolsSyndicateInfogroup( String entity_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("destructive",destructive);
    return doCurl("GET","/tools/syndicate/infogroup",p);
  }


  /**
   * Fetch the entity and convert add it to Judy's Book
   *
   *  @param entity_id - The entity_id to fetch
   *  @param destructive - Add the entity or simulate adding the entity
   *  @return - the data from the api
  */
  public String GETToolsSyndicateJudysbook( String entity_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("destructive",destructive);
    return doCurl("GET","/tools/syndicate/judysbook",p);
  }


  /**
   * Fetch the entity and convert it to Google KML format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateKml( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/kml",p);
  }


  /**
   * Syndicate database to localdatabase.com
   *
   *  @param entity_id - The entity_id to fetch
   *  @param destructive - Add the entity or simulate adding the entity
   *  @return - the data from the api
  */
  public String GETToolsSyndicateLocaldatabase( String entity_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("destructive",destructive);
    return doCurl("GET","/tools/syndicate/localdatabase",p);
  }


  /**
   * Fetch the entity and convert it to Nokia NBS CSV format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateNokia( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/nokia",p);
  }


  /**
   * Post an entity to OpenStreetMap
   *
   *  @param entity_id - The entity_id to fetch
   *  @param destructive - Add the entity or simulate adding the entity
   *  @return - the data from the api
  */
  public String GETToolsSyndicateOsm( String entity_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("destructive",destructive);
    return doCurl("GET","/tools/syndicate/osm",p);
  }


  /**
   * Syndicate an entity to ThomsonLocal
   *
   *  @param entity_id - The entity_id to fetch
   *  @param destructive - Add the entity or simulate adding the entity
   *  @return - the data from the api
  */
  public String GETToolsSyndicateThomsonlocal( String entity_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("destructive",destructive);
    return doCurl("GET","/tools/syndicate/thomsonlocal",p);
  }


  /**
   * Fetch the entity and convert it to TomTom XML format
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateTomtom( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/tomtom",p);
  }


  /**
   * Fetch the entity and convert it to YALWA csv
   *
   *  @param entity_id - The entity_id to fetch
   *  @return - the data from the api
  */
  public String GETToolsSyndicateYalwa( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("GET","/tools/syndicate/yalwa",p);
  }


  /**
   * Fetch the entity and convert add it to Yassaaaabeeee!!
   *
   *  @param entity_id - The entity_id to fetch
   *  @param destructive - Add the entity or simulate adding the entity
   *  @return - the data from the api
  */
  public String GETToolsSyndicateYasabe( String entity_id, String destructive) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("destructive",destructive);
    return doCurl("GET","/tools/syndicate/yasabe",p);
  }


  /**
   * Test to see whether this supplied data would already match an entity
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
  public String GETToolsTestmatch( String name, String building_number, String branch_name, String address1, String address2, String address3, String district, String town, String county, String province, String postcode, String country, String latitude, String longitude, String timezone, String telephone_number, String additional_telephone_number, String email, String website, String category_id, String category_type, String do_not_display, String referrer_url, String referrer_name) {
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
    return doCurl("GET","/tools/testmatch",p);
  }


  /**
   * Send a transactional email via Adestra or other email provider
   *
   *  @param email_id - The ID of the email to send
   *  @param destination_email - The email address to send to
   *  @param email_supplier - The email supplier
   *  @return - the data from the api
  */
  public String GETToolsTransactional_email( String email_id, String destination_email, String email_supplier) {
    Hashtable p = new Hashtable();
    p.Add("email_id",email_id);
    p.Add("destination_email",destination_email);
    p.Add("email_supplier",email_supplier);
    return doCurl("GET","/tools/transactional_email",p);
  }


  /**
   * Upload a file to our asset server and return the url
   *
   *  @param filedata
   *  @return - the data from the api
  */
  public String POSTToolsUpload( String filedata) {
    Hashtable p = new Hashtable();
    p.Add("filedata",filedata);
    return doCurl("POST","/tools/upload",p);
  }


  /**
   * Find a canonical URL from a supplied URL
   *
   *  @param url - The url to process
   *  @param max_redirects - The maximum number of reirects
   *  @return - the data from the api
  */
  public String GETToolsUrl_details( String url, String max_redirects) {
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
  public String GETToolsValidate_email( String email_address) {
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
  public String GETToolsValidate_phone( String phone_number, String country) {
    Hashtable p = new Hashtable();
    p.Add("phone_number",phone_number);
    p.Add("country",country);
    return doCurl("GET","/tools/validate_phone",p);
  }


  /**
   * Deleting a traction
   *
   *  @param traction_id
   *  @return - the data from the api
  */
  public String DELETETraction( String traction_id) {
    Hashtable p = new Hashtable();
    p.Add("traction_id",traction_id);
    return doCurl("DELETE","/traction",p);
  }


  /**
   * Fetching a traction
   *
   *  @param traction_id
   *  @return - the data from the api
  */
  public String GETTraction( String traction_id) {
    Hashtable p = new Hashtable();
    p.Add("traction_id",traction_id);
    return doCurl("GET","/traction",p);
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
  public String POSTTraction( String traction_id, String trigger_type, String action_type, String country, String email_addresses, String title, String body, String api_method, String api_url, String api_params, String active, String reseller_masheryid, String publisher_masheryid, String description) {
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
   * Fetching active tractions
   *
   *  @return - the data from the api
  */
  public String GETTractionActive() {
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
  public String PUTTransaction( String entity_id, String user_id, String basket_total, String basket, String currency, String notes) {
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
  public String GETTransaction( String transaction_id) {
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
  public String POSTTransactionAuthorised( String transaction_id, String paypal_getexpresscheckoutdetails) {
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
  public String GETTransactionBy_paypal_transaction_id( String paypal_transaction_id) {
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
  public String POSTTransactionCancelled( String transaction_id) {
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
  public String POSTTransactionComplete( String transaction_id, String paypal_doexpresscheckoutpayment, String user_id, String entity_id) {
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
  public String POSTTransactionInprogress( String transaction_id, String paypal_setexpresscheckout) {
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
   *  @param last_flatpack - Last visited flatpack (used for admin deep linking)
   *  @param trust
   *  @param creation_date
   *  @param user_type
   *  @param social_network
   *  @param social_network_id
   *  @param reseller_admin_masheryid
   *  @param group_id
   *  @param admin_upgrader
   *  @param opt_in_marketing
   *  @return - the data from the api
  */
  public String POSTUser( String email, String user_id, String first_name, String last_name, String active, String last_flatpack, String trust, String creation_date, String user_type, String social_network, String social_network_id, String reseller_admin_masheryid, String group_id, String admin_upgrader, String opt_in_marketing) {
    Hashtable p = new Hashtable();
    p.Add("email",email);
    p.Add("user_id",user_id);
    p.Add("first_name",first_name);
    p.Add("last_name",last_name);
    p.Add("active",active);
    p.Add("last_flatpack",last_flatpack);
    p.Add("trust",trust);
    p.Add("creation_date",creation_date);
    p.Add("user_type",user_type);
    p.Add("social_network",social_network);
    p.Add("social_network_id",social_network_id);
    p.Add("reseller_admin_masheryid",reseller_admin_masheryid);
    p.Add("group_id",group_id);
    p.Add("admin_upgrader",admin_upgrader);
    p.Add("opt_in_marketing",opt_in_marketing);
    return doCurl("POST","/user",p);
  }


  /**
   * With a unique ID address an user can be retrieved
   *
   *  @param user_id
   *  @return - the data from the api
  */
  public String GETUser( String user_id) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    return doCurl("GET","/user",p);
  }


  /**
   * Is this user allowed to edit this entity
   *
   *  @param entity_id
   *  @param user_id
   *  @return - the data from the api
  */
  public String GETUserAllowed_to_edit( String entity_id, String user_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    p.Add("user_id",user_id);
    return doCurl("GET","/user/allowed_to_edit",p);
  }


  /**
   * With a unique email address an user can be retrieved
   *
   *  @param email
   *  @return - the data from the api
  */
  public String GETUserBy_email( String email) {
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
  public String GETUserBy_groupid( String group_id) {
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
  public String GETUserBy_reseller_admin_masheryid( String reseller_admin_masheryid) {
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
  public String GETUserBy_social_media( String name, String id) {
    Hashtable p = new Hashtable();
    p.Add("name",name);
    p.Add("id",id);
    return doCurl("GET","/user/by_social_media",p);
  }


  /**
   * Downgrade an existing user
   *
   *  @param user_id
   *  @param user_type
   *  @return - the data from the api
  */
  public String POSTUserDowngrade( String user_id, String user_type) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    p.Add("user_type",user_type);
    return doCurl("POST","/user/downgrade",p);
  }


  /**
   * Removes group_admin privileges from a specified user
   *
   *  @param user_id
   *  @return - the data from the api
  */
  public String POSTUserGroup_admin_remove( String user_id) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    return doCurl("POST","/user/group_admin_remove",p);
  }


  /**
   * Log user activities into MariaDB
   *
   *  @param user_id
   *  @param action_type
   *  @param domain
   *  @param time
   *  @return - the data from the api
  */
  public String POSTUserLog_activity( String user_id, String action_type, String domain, String time) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    p.Add("action_type",action_type);
    p.Add("domain",domain);
    p.Add("time",time);
    return doCurl("POST","/user/log_activity",p);
  }


  /**
   * List user activity times within given date range (between inclusive from and exclusive to)
   *
   *  @param user_id
   *  @param action_type
   *  @param from
   *  @param to
   *  @return - the data from the api
  */
  public String GETUserLog_activityList_time( String user_id, String action_type, String from, String to) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    p.Add("action_type",action_type);
    p.Add("from",from);
    p.Add("to",to);
    return doCurl("GET","/user/log_activity/list_time",p);
  }


  /**
   * Retrieve list of entities that the user manages
   *
   *  @param user_id
   *  @return - the data from the api
  */
  public String GETUserManaged_entities( String user_id) {
    Hashtable p = new Hashtable();
    p.Add("user_id",user_id);
    return doCurl("GET","/user/managed_entities",p);
  }


  /**
   * Removes reseller privileges from a specified user
   *
   *  @param user_id
   *  @return - the data from the api
  */
  public String POSTUserReseller_remove( String user_id) {
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
  public String DELETEUserSocial_network( String user_id, String social_network) {
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
  public String GETViewhelper( String database, String designdoc, String view, String doc) {
    Hashtable p = new Hashtable();
    p.Add("database",database);
    p.Add("designdoc",designdoc);
    p.Add("view",view);
    p.Add("doc",doc);
    return doCurl("GET","/viewhelper",p);
  }


  /**
   * Converts an Entity into webcard JSON and then doing a PUT /webcard/json
   *
   *  @param entity_id - The entity to create on webcard
   *  @return - the data from the api
  */
  public String POSTWebcard( String entity_id) {
    Hashtable p = new Hashtable();
    p.Add("entity_id",entity_id);
    return doCurl("POST","/webcard",p);
  }


}


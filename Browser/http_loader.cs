using System.IO;

namespace H3ml.Browser
{
    public class http_loader
    {
        object _curl;
        string _url;

        public http_loader()
        {
            m_curl = curl_easy_init();
            curl_easy_setopt(m_curl, CURLOPT_FOLLOWLOCATION, 1L);
            curl_easy_setopt(m_curl, CURLOPT_TCP_KEEPALIVE, 1L);
            curl_easy_setopt(m_curl, CURLOPT_TCP_KEEPIDLE, 120L);
            curl_easy_setopt(m_curl, CURLOPT_TCP_KEEPINTVL, 60L);
            curl_easy_setopt(m_curl, CURLOPT_WRITEFUNCTION, http_loader::curlWriteFunction);
        }

        public Stream load_file(string url)
        {
            _url = url;
            var stream = new MemoryStream();
            if (_curl != null)
            {
                curl_easy_setopt(m_curl, CURLOPT_URL, url.c_str());
                curl_easy_setopt(m_curl, CURLOPT_WRITEDATA, &stream);
                curl_easy_perform(m_curl);
                char* new_url = NULL;
                if (curl_easy_getinfo(m_curl, CURLINFO_EFFECTIVE_URL, &new_url) == CURLE_OK)
                {
                    if (new_url)
                    {
                        m_url = new_url;
                    }
                }
            }

            return stream;
        }

        public string url => _url;

        static size_t curlWriteFunction(void* ptr, size_t size, size_t nmemb, void* stream)
        {
            Glib::RefPtr<Gio::MemoryInputStream>* s = (Glib::RefPtr<Gio::MemoryInputStream>*)stream;
            (*s)->add_data(ptr, size * nmemb);
            return size * nmemb;
        }
    }
}
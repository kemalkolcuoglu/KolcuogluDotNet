![](../images/blog/python-ile-log-kayitlari-olusturma/undraw_logistics_x4dc.png)

Hata ayıklamak her zaman çok kolay olmayabiliyor. Her programlama dili için hata ayıklama araçları mevcut olsa bile günün birinde canlı sunucu üzerinde bu işlemi gerçekleşmeniz gerekebiliyor. Peki böyle bir durumda koddaki her yere `print` yazarak mı sorunları çözeceğiz? Elbette bu işin en iyi yolu log kayıtları oluşturma.Bu yazımda sizlerle python ile nasıl log kayıtları oluşturabileceğimiz ve log kayıtlarını nasıl yönetebileceğimizi inceleyeceğiz.

Canlıda çalışan bir kodun her yerine `print` fonksiyonu yazmak elbette çözüm için yardımcı olabilir. Ancak hiç profesyonel durmuyor. Özellikle python gibi kod bloklarında boşlukların kullanıldığı bir dilde fazladan bir boşluk koymanız kodu patlatabilir. Yazım hataları gibi çok yaşanan durumlardan bahsetmiyorum bile.
'Peki `print` fonksiyonunu geliştirme esnasında kullansak ne olur?' diye sorabilirsiniz. Ancak her script o an çalıştırılmak istenmeyebilir ya da bir cronjob'a bağlı olarak çalıştığı için farklı sorunlara yol açabilir. Bu yüzden geliştirme esnasında uygulanması gereken en iyi çözüm logging altyapısı kurmaktır.

Python built-in modullerinden birisi olan 'logging' modülünde loglama işlemleri ile alakalı birçok ayar bulunmakta.

- **Seviyeler:**

Logging işleminde öncelikli olarak bilinmesi gereken konulardan birisi log seviyeleridir. Bu seviyeler sırasıyla **DEBUG (10)**, **INFO (20)**, **WARNING (30)**, **ERROR (40)** ve **CRITICAL (50)** şeklindedir. Seviyelerin yanındaki değerler onların ağırlık değeridir. Log seviyeleri bulunduğu ağırlık değerini ve üstünü değerlendirir.

**DEBUG ->** Kod içerisindeki yazılımcının debug yapmasını kolaylaştıran mesajları içerir. Gelen verinin değeri, tipi, hangi uçnokta ile bağlantı oluşturuldu vb. içerikteki bilgiler DEBUG seviyesinde yazılmalıdır.

```log
DEBUG: ## SCRIPT BASLADI ##
DEBUG: type(a) = <class 'dict'>
DEBUG: a = {'Merhaba': 'Dunya'}
DEBUG: ## SCRIPT SONLANDI ##
```

**INFO ->** Asıl yapılacak olan görevin durumu, sonucu vb bilgiler INFO seviyesinde yazılmalıdır.

```log
INFO: Veritabanına veriler kaydedildi.
INFO: Kullanıcı yetkilendirme başarılı
```

**WARNING ->** Bir hata olmayan ancak hata teşkil edebilecek durumlara yol açabilecek mesajlar WARNING seviyesinde yazılmalıdır.

```log
WARNING: Beklenen veri sayısı: 2 - Gelen veri sayısı: 1
WARNING: Uçnokta HTTPS desteklememektedir
```

**ERROR ->** Artık hataları yazmaya başladığımız kısıma geldik. Öngörülebilir hataların yaşanabileceği kısımlara mesajlar ERROR seviyesinde yazılmalıdır.

```log
ERROR: Yetkilendirme hatası - Kullanıcı adı ya da şifre geçersiz
ERROR: '15' değeri 0'a bölünemez
ERROR: Veritabanı bağlantı hatası - Veritabanına ulaşılamıyor
ERROR: '/home/kemal/logging.py' dosyası bulunamadı
```

**CRITICAL ->** Öngöremediğimiz ve sistemde büyük sorunlara yol açabilecek durumların mesajları CRITICAL seviyesinde yazılmalıdır.

```log
CRITICAL: Disk kullanımı 95% seviyesinde
```

Seviyeleri öğrendiğimize göre artık kodla bunları nasıl uygulayacağımızı öğrenebiliriz. Örnek üzerinden inceleyelim:

```python log_training.py
import logging

try:
    value = 1 / 0 
except Exception as ex:
    logging.error(ex)
# -----------------------------------------------
# Output:
# ERROR:root:division by zero
```

Görüldüğü gibi, `print` fonksiyonu kullanmaksızın hatayı görüntüleyebiliyoruz. Bunun `print` fonksiyonundan ne farkı kaldı diyorsanız örneğimizde bir ekleme yapalım.

```python log_training.py
import logging

logging.basicConfig(
    format='%(asctime)s - %(levelname)s - %(funcName)s - %(lineno)d - %(message)s' # Datetime - Log Level - Fonksiyon adı - Satır Numarası - Mesaj
)

try:
    value = 1 / 0 
except Exception as ex:
    logging.error(ex)
# -----------------------------------------------
# Output:
# 2020-10-02 22:55:05,565 - ERROR - <module> - 10 - division by zero
```

Bi anda error mesajımız bambaşka bir hal aldı. Hangi saatte, hangi tipte, hangi fonksiyonda (kod bloğu bir fonksiyon içinde olmadığı için `<module>` şeklinde yazılmıştır), hangi satırda yaşandığını ve neden hatanın oluştuğunu detaylı bir şekilde vermekte. Elbette `format` değeri içerisinde **process id**, **kullanıcı adı**, **dosya yolu** vb. daha birçok değer eklenilebilir.

Peki log dosyasını nasıl oluşturacağız diyorsanız bir sonraki örneği inceleyelim.

```python log_training.py
import logging

logging.basicConfig(
    filename='example.log',
    filemode='a',
    format='%(asctime)s - %(levelname)s - %(funcName)s - %(lineno)d - %(message)s',
    level=logging.DEBUG
)

try:
    value = 1 / 0 
except Exception as ex:
    logging.error(ex)
# -----------------------------------------------
# Output:
#
```

> Eğer ki her çalıştırma log dosyasındaki eski logların silinmesini istiyorsanız `filemode` değerini **w** yazınız, istemiyorsanız **a** yazınız. (Tavsiye edilen `append - a` modunda kalmasıdır.)

-Ee ne oldu? Hiçbir çıktı vermedi bize?

Tabiki vermedi :blush: Çünkü mesajı bir dosyaya kaydetti. Artık sorun var mı diye dosyaya bakarak anlayabiliriz.

**example.log** Dosyasının İçeriği:
```log example.log
2020-10-02 23:01:46,748 - ERROR - <module> - 13 - division by zero
```

-İyi ama resmen seviye düştük. Hem ekranda yazdırıp hemde dosyaya yazdıramaz mıyız?

Elbette yazdırabiliriz :blush: onu da düşünmüşler.

Bu kez iş biraz karmaşıklaşıyor ve `logging.handler` nesnesi kullanmamız gerekecek.


```python log_training.py
import logging

simple_format = '%(asctime)s - %(name)s - %(levelname)s - %(message)s'
detailed_format = '%(asctime)s - %(levelname)s - %(funcName)s - %(lineno)d - %(message)s'

logging.basicConfig(
    filename='example.log',
    filemode='a',
    format=detailed_format,
    level=logging.ERROR
)
logger = logging.getLogger('simple_example')
ch = logging.StreamHandler()
ch.setLevel(logging.DEBUG)
formatter = logging.Formatter(simple_format)
ch.setFormatter(formatter)
logger.addHandler(ch)

logger.error('error message')
logger.critical('critical message')
# -----------------------------------------------
# Output:
# 2020-10-02 23:21:14,667 - simple_example - ERROR - error message
# 2020-10-02 23:21:14,667 - simple_example - CRITICAL - critical message
```

```log example.log
2020-10-02 23:23:33,821 - ERROR - <module> - 19 - error message
2020-10-02 23:23:33,821 - CRITICAL - <module> - 20 - critical message
```

Görüldüğü gibi! Artık hem dosyaya yazdırıyoruz hem de console çıktısı olarak görüntüleyebiliyoruz. Fakat maalesef ki bu da yeterli değil :) Asıl yapmamız gereken o log dosyasını yönetebilmektedir.

Log dosyasının yönetimi çok önemlidir. Zamanla log dosyasının şişmiş bir şekilde diskte çok büyük bir yer kapladığını görmek mümkün. Bunun önüne geçebilmek için Python farklı handler'lar sunuyor.

> Bu kısım [How To Logging - Useful Handlers](https://docs.python.org/3/howto/logging.html#useful-handlers) makalesinin Türkçe çevirisidir. Lütfen orjinal metne göz atınız.

1. **StreamHandler:** Mesajlar tıpkı bir nesne gibi stream'e gönderilir. (En kaba tabiriyle console'da output olarak yazılır.) **Bknz: Stdin, Stdout, Stderr**

2. **FileHandler:** Mesajlar diskteki bir dosyaya yazılır.

3. **RotatingFileHandler:** Mesajlar bir dosyaya yazılır. Ancak dosyaya en fazla tanımlanmış olan max byte değeri kadar veri yazılabilir. Eğer boyut aşılırsa tanımlanmış olan max dosya sayısı kadar dosya oluşturulabilir. Daha eskiler silinir.

4. **TimedRotatingFileHandler:** Mesajlar bir dosyaya yazılır. Ancak dosyalar belirlenen tarihe kadar saklanır. Tanımlanan tarihi geçen dosyalar diskten silinir.

5. **SocketHandler:** Kayıtlar TCP/IP socketlerine iletilir.

6. **DatagramHandler:** Kayıtlar UDP socketlerine iletilir.

7. **SMTPHandler:** Kayıtlar belirlenen adrese e-posta yoluyla iletilir.

8. **SysLogHandler:** Kayıtlar Unix syslog daemon'a iletilir.

9. **NTEventLogHandler:** Kayıtlar Windows NT/2000/XP loglarına iletilir.

10. **MemoryHandler:** Kayıtlar belirli kriterler karşılandığında boşaltılan bellekteki bir arabelleğe iletilir.

11. **HTTPHandler:** Kayıtlar iletileri GET veya POST methodları kullanarak bir HTTP sunucusuna gönderir.

12. **WatchedFileHandler:** Kayıtlar oturum açtıkları dosyayı izler. Dosya değişirse kapatılır ve dosya adı kullanılarak yeniden açılır. Bu işlem yalnızca Unix benzeri sistemlerde kullanışlıdır; Windows, kullanılan temel mekanizmayı desteklemez.


En özet haliyle pythonda logging işleminin nasıl gerçekleştiğinden bahsetmek istedim. Bir başka yazımda **RotatingFileHandler** nasıl kullanılır bundan bahesedeceğim.

Yazılım ile kalın! :blush:

---
Manşet Görseli UnDraw'dan alınmıştır -> [Undraw](https://undraw.co/illustrations)

**Referanslar:**

- [How To Logging - Useful Handlers](https://docs.python.org/3/howto/logging.html)

- [Python Logging](https://docs.python.org/3/library/logging.html)

- [Python Logging.Config](https://docs.python.org/3/library/logging.config.html)

- [Python Logging.Handlers](https://docs.python.org/3/library/logging.handlers.html)
![](../images/blog/python-rotatingfilehandler-kullanimi/undraw_heavy_box_agqi.png)

Geçtiğimiz hafta [Python ile Log Kayıtları Oluşturma](http://kolcuoglu.net/Blog/python-ile-log-kayitlari-olusturma-5) adlı bir makale yazarak nasıl log kayıtları oluşturabileceğimizi ve bunları nasıl yönetecebileceğimiz üzerine değinmiştim. Bu yazımla birlikte logging handlerlarından birisi olan **RotatingFileHandler**'ı nasıl kullanabileceğimiz üzerine konuşacağız.

Öncelikle RotatingFileHandler neydi bir hatırlayalım.

>**RotatingFileHandler:** Mesajlar bir dosyaya yazılır. Ancak dosyaya en fazla tanımlanmış olan max byte değeri kadar veri yazılabilir. Eğer boyut aşılırsa tanımlanmış olan max dosya sayısı kadar dosya oluşturulabilir. Daha eskiler silinir.

Yani yazılacak olan log kayıtları bir dosyada saklanıyor. Ancak bu dosyaların daha önceden belirlenmiş bir üst limit dosya boyutu mevcut. Eğer ki bu boyut aşılırsa eski kayıtlar saklanarak farklı bir dosyaya aktarılıyor, yeni kayıtlar ise aynı dosyaya yazılmaya devam ediyor. Bu örneği linux üzerinde inceleyecek olursak:

```bash
 turam@Turam-PC /var/log$ cd /var/log && ls -alh | grep vbox-setup
-rw-r--r--  1 root              root             102 Eyl 22 00:18 vbox-setup.log
-rw-r--r--  1 root              root             102 Eyl 17 11:04 vbox-setup.log.1
-rw-r--r--  1 root              root             102 Eyl  5 01:23 vbox-setup.log.2
-rw-r--r--  1 root              root             102 Tem 22 00:25 vbox-setup.log.3
-rw-r--r--  1 root              root             102 Tem  2 21:18 vbox-setup.log.4
```

Görmüş olduğunuz gibi her dosya tarihten bağımsız bir şekilde dosya boyutuna göre saklanarak log kayıtları tutulmakta.

Bir önceki makalede log kayıtlarının oluşturulması ile alakalı örnekleri `logging.basicConfig` methodu ile gerçekleştirmiştik. Ancak konfigrasyon girmenin tek yolu bu methodu kullanmak değildir. Python'un `logging.config` modülü bu noktada devreye girerek bize üç farklı yok ile konfigrasyon dosyası oluşturabilme imkanı sağlıyor. Dosya formatları klasik conf dosyası şeklinde ya da json ve yaml dosyası şeklinde oluşturulabilir. Ben bu noktada json dosyası kullanılmasını tavsiye ederim.

>Json dosyasının yapısı Python'nın veri tiplerinden birisi olan dict ile aynı formata sahiptir. Aynı zamanda json dosyası okunabilirliği açısından diğer iki dosya formatından daha ön plana çıkmatadır.

Şimdi bir konfigrasyon dosyası oluşturalım.

```json logging.json
{
  "version": 1,
  "formatters": {
    "simple": {
      "class": "logging.Formatter",
      "format": "%(asctime)s - %(name)s - %(levelname)s - %(message)s"
    },
    "detailed": {
      "class": "logging.Formatter",
      "format": "%(asctime)s - %(name)s - %(levelname)s - %(filename)s - %(funcName)s - %(lineno)d - %(message)s"
    },
    "full-description": {
      "class": "logging.Formatter",
      "format": "%(asctime)s - %(name)s - %(levelname)s - %(levelno)s - %(module)s - %(pathname)s - %(filename)s - %(funcName)s - %(lineno)d - %(processName)s - %(process)d - %(threadName)s - %(thread)d - %(message)s"
    }
  },
  "handlers": {
    "console": {
      "class": "logging.StreamHandler",
      "level": "DEBUG",
      "formatter": "detailed",
      "stream": "ext://sys.stdout"
    },
    "info_file_handler": {
      "class": "logging.handlers.RotatingFileHandler",
      "level": "INFO",
      "formatter": "simple",
      "filename": "python_script.log",
      "maxBytes": 10485760,
      "backupCount": 10,
      "encoding": "utf8"
    },
    "error_file_handler": {
      "class": "logging.handlers.RotatingFileHandler",
      "level": "ERROR",
      "formatter": "detailed",
      "filename": "python_script_error.log",
      "maxBytes": 10485760,
      "backupCount": 10,
      "encoding": "utf8"
    }
  },
  "root": {
    "level": "DEBUG",
    "handlers": [
      "console",
      "info_file_handler",
      "error_file_handler"
    ]
  }
}
```

Bu konfigrasyon dosyasında üç farklı format ekledim. Bunlar `simple`, `detailed` ve `full-description`. Yani adlarındanda anlayacağınız üzere her bir formatta yazılan içeriğin detayı artmakta.

Üç farklı handler kullanılarak farklı seviyelerdeki mesajlara göre farklı davranışlar sergilenecektir. DEBUG mesajları yalnızca script çalıştırıldığında ekrana yazıdırılırken, INFO ve daha üst seviyedeki mesajlar `python_script.log` dosyasına yazdırılacak. Son olarak `ERROR` ve `CRITICAL` mesajları `python_script_error.log` dosyasına yazdırılacaktır.

Asıl RotatingFileHandler kullanmamızdaki sebep olan dosya boyutu ise `maxBytes` değerine yazılıyor. Her iki handler içinde 10485760 byte yani 10 MB'lık bir dosya boyutu tanımladım. Eğer ki log mesajları 10 MB'ı aşarsa en fazla 10 dosya olacak şekilde saklanmaya devam edecektir. Bu değer ise `backupCount` değeri içinde verilmiştir. Yani 11. dosya oluşmaksızın en eski log kayıtları silinecektir.

Konfigrasyon dosyamızı yazdığımıza göre bunu python'da nasıl kullanabileceğimize göz atalım.

> Not: logging.json dosyası içerisinde verilen değerlerin tamamı `logging.basicConfig` methodu ile tanımlanabilir. Yine de ayrı bir dosya olarak oluşturmanız tavsiye edilir.

```python
import os, sys
import json
import logging
import logging.config
import logging.handlers

def setup_logging(default_path: str ='logging.json', default_level: int = logging.DEBUG):
    BASE_DIR = os.path.dirname(os.path.abspath(__file__)) # Önemli! Eğer ki dosya yolu tanımlanmaz konfig dosyası okunamaz.
    path = os.path.join(BASE_DIR, default_path)
    if os.path.exists(path):                      # Konfigrasyon dosyasının var olup olmadığı kontrol ediyoruz
        with open(path, 'rt') as config_file:     # logging.json dosyası with open ile okunuyor
            config = json.load(config_file)       # json formatında olduğu için json.load methodu kullanılıyor
        logging.config.dictConfig(config)         # json formatında oluşturulmuş konfigrasyonlar için dictConfig dosyası kullanılır
    else:
        logging.basicConfig(level=default_level)  # Eğer ki dosya yoksa default ayarlar kullanılacak ve bütün mesajlar ekrana yazdırılacaktır
```

Bu kadar basit! artık deneme yapabiliriz.

```python
import os, sys
import json
import logging
import logging.config
import logging.handlers

def setup_logging(default_path: str ='logging.json', default_level: int = logging.DEBUG):
    BASE_DIR = os.path.dirname(os.path.abspath(__file__))
    path = os.path.join(BASE_DIR, default_path)
    if os.path.exists(path):
        with open(path, 'rt') as config_file:
            config = json.load(config_file)
        logging.config.dictConfig(config)
        logging.info('Log Yapılandırması Tamamlandı')
    else:
        logging.basicConfig(level=default_level)
        logging.error('Log Yapılandırması Tamamlanamadı - Default değerler kullanılacak')

def try_dividing_by_zero():
    logging.debug("1 Sayısı 0'a bölünmeye çalışılıyor.")
    try:
        value = 1 / 0
        logging.info('Sonuç Başarılı!')
    except Exception as ex:
        logging.error(ex)

setup_logging()
try_to_divide_zero()
# -----------------------------------------------
# Output:
# 2020-10-07 22:41:08,364 - root - INFO - deneme.py - setup_logging - 14 - Log Yapılandırması Tamamlandı
# 2020-10-07 22:41:08,364 - root - DEBUG - deneme.py - try_dividing_by_zero - 20 - 1 Sayısı 0'a bölünmeye çalışılıyor.
# 2020-10-07 22:41:08,364 - root - ERROR - deneme.py - try_dividing_by_zero - 25 - division by zero
```

**python_script.log** Dosyasının İçeriği:

```log python_script.log
2020-10-07 22:41:08,364 - root - INFO - Log Yapılandırması Tamamlandı
2020-10-07 22:41:08,364 - root - ERROR - division by zero
```

**python_script_error.log** Dosyasının İçeriği:

```log python_script.log
2020-10-07 22:41:08,364 - root - ERROR - deneme.py - try_dividing_by_zero - 25 - division by zero
```

Görmüş olduğunuz gibi; hem output olarak hemde dosyaya yazdırılarak log kayıtlarını oluşturmuş olduk.

**Sıkça Sorulan Sorular:**

+ Info ve Error olarak iki dosyamız var ama Info içerisinde Error mesajlarda mevcut. Bu neden oluyor?

Python Logging mekanizmasında mesajlar; her seviye kendi ve üst seviyedeki mesajları kabul eder. Alt seviye mesajlar reddedilir. Bu yüzden python_script.log dosyamızda error mesajlarını görebiliyorken debug mesajlarını göremiyoruz.

+ Bunu yapılandırmanın bir yolu yok mu?

>Elbette var! Ancak biraz daha uğraştırıcı ve ek fonksiyon kullanılması gerekir. Lütfen şu stackoverflow sayfasını inceleyin -> [https://stackoverflow.com/questions/59120160/log-only-to-a-file-and-not-to-screen-for-logging-debug](https://stackoverflow.com/questions/59120160/log-only-to-a-file-and-not-to-screen-for-logging-debug)

+ Birden fazla script var ve her birisi için bu methodu eklemek istemiyorum. Ne yapabilirim?

>Cevap bir class oluşturmak! LoggingUtility.py adında bir dosya oluşturun ve logger kullanarak her bir scriptte bu logger'ı kullanın. En iyisi örnek üzerinden anlatalım.

```python LoggingUtility.py
import os, sys
import json
import logging
import logging.config
import logging.handlers


class LoggingUtility:
    def __init__(self, default_path: str = 'logging.json', default_level: int = logging.DEBUG):
        BASE_DIR = os.path.dirname(os.path.abspath(__file__))
        path = os.path.join(BASE_DIR, default_path)
        if os.path.exists(path):
            with open(path, 'rt') as config_file:
                config = json.load(config_file)
            logging.config.dictConfig(config)
        else:
            logging.basicConfig(level=default_level)


if __name__ == '__main__':
    loggingUtility = LoggingUtility()
    logger = logging.Logger('loggingUtility')
```

```python
import os, sys
import json
import logging
import LoggingUtility
log = logging.getLogger('LoggingUtility')

def try_dividing_by_zero():
    log.debug("1 Sayısı 0'a bölünmeye çalışılıyor.")
    try:
        value = 1 / 0
        log.info('Sonuç Başarılı!')
    except Exception as ex:
        log.error(ex)

try_dividing_by_zero()
```

Görmüş olduğunuz gibi! Çıktılar bir önceki örnektekiyle aynı. Artık birden fazla script'e yalnızca `log = logging.getLogger('LoggingUtility')` satırını ekleyerek sorun çözüldü.

Bu yazımla birlikte RotatingFileHandler üzerine örnekler vererek konuyu anlatmaya çalıştım. Umarım projelerinizde kullanabileceğiniz yararlı bir kaynak olur :blush:

Yazılım ile kalın! :blush:

---
Manşet Görseli UnDraw'dan alınmıştır -> [Undraw](https://undraw.co/illustrations)

**Referanslar:**

- [How To Logging - Useful Handlers](https://docs.python.org/3/howto/logging.html)

- [Python Logging](https://docs.python.org/3/library/logging.html)

- [Python Logging.Config](https://docs.python.org/3/library/logging.config.html)

- [Python Logging.Handlers](https://docs.python.org/3/library/logging.handlers.html)
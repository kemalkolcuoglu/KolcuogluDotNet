![](../images/blog/phpden-pythona-gecis-bolum-0/undraw_programmer_imem.png)

İşyerinde aldığım iş üzerine eski bir PHP scriptini Python scriptine dönüştürmek üzerine kollarımı sıvadım. Tam bu esnada olanlar oldu :persevere:
Yaşadığım zorlukları ve diller arası geçişteki farklılıkları özet halinde bu yazı dizisinde sizlerle paylaşacağım.

## Diller Arası Geçiş

Elbetteki hali hazırda çalışan bir kodu farklı dille yeniden yazma fikri ilk başta tuhaf gelebilir.
Ancak takım üyelerinin yeni dile daha çok yatkın olması, oluşturulan yeni projede tek bir dil kullanılmak istenmesi,
eski yapıları yenilemek ve düzenlemelerin kolaylaştırılması gibi faktörler nedeniyle diller arası geçişler yaşanabilmektedir.

Sözleri değil kodları konuşturalım :)


## 1. Regex

PHP kodlarında regex olmazsa olmaz diyebileceğimiz bir özellik. Özellikle *nix sistemi üzerinde çalışan scriptlerde dosyaların içerisinde verileri okumak, threadleri takip etmek gibi konular için regex vazgeçilmez bir özelliktir. Ancak PHP'de ve Python'da regex kullanımında bazı farklılıklar bulunmaktadır.

Aşağıdaki örnekte en çok kullanılan fonksiyonardan birisi olan `preg_match` kullanılmıştır. Özetle, elimizde bulunan text içerisinde pattern'i arayarak değerler bulunursa bir array olarak döndürür.

``` php
$text = 'Daily updates available'
$result = preg_match("/daily/i",$text)
# -----------------------------------------------
# $result: array( 0 => 'Daily' )
```

Bu kodun Python karşılığı aşağıdaki gibidir.

``` python
import re

text = 'Daily updates available'
result = re.match('daily', text, re.IGNORECASE) # re.IGNORECASE yerine re.I' da alternatif olarak kullanılabilir.
# -----------------------------------------------
# result: <re.Match object; span=(0, 5), match='Daily'>
# result.group(): 'Daily'
```

> Görmüş olduğunuz gibi PHP kodu bir `array` döndürürken, Python kodu bir `re.match` objesi döndürmektedir. İçindeki veriyi çekebilmek için `result.group()` methodu kullanıyoruz.

> Ayrıca PHP kodunda `pattern` parametresinde regex kuralı olan `/` karakterlerini ve flag'i elle girdik. Ancak Python kodunda `/` karakteri Python tarafından otomatik olarak eklenir. Eğer PHP'de yazıldığı gibi kullanırsanız muhtemelen match objesi boş dönecektir. Flag'ler içinde `re.match()` methodunun üçüncü parametresine eklemeniz Python tarafından zorunlu kılınmıştır.

**Dikkat!**
PHP kodunda `preg_grep` gibi regex fonksiyonu `array` içerisindeki değerler üzerinde çalışabilme imkanı vermektedir. Ancak bu fonksiyonun Python alternatifi olan `re.search` methodu yalnızca `string` değerler üzerinde çalışmaktadır.

``` php
$values = array( "foo" => "value1", "bar" => "value2", "var" => "not a val");
$result = preg_grep('/value+/', $values);
print_r($result);
# -----------------------------------------------
# Output:
# Array
# (
#     [foo] => value1
#     [bar] => value2
# )
```
Python kodu içerisinde regex'e ait `+` operatörü kullanacağımız için string'in önüne `r` karakteri eklenmelidir.
Ancak aranacak değerleri bir dict olarak gönderdiğimiz için kod hata verecektir. Aynı durum list içinde geçerli olup regex methodlarında aranacak değerin `string` yada `byte-string` olması gerektiğini unutmayınız.

``` python
import re

values = {"foo": "value1", "bar": "value2", "var": "not a val"}
result = re.search(r'value+', values)
# -----------------------------------------------
# Output:
Traceback (most recent call last):
  File "<stdin>", line 1, in <module>
  File "/usr/lib/python3.8/re.py", line 199, in search
    return _compile(pattern, flags).search(string)
TypeError: expected string or bytes-like object
```

**Offical Docs:**

- [PHP Regex](https://www.php.net/manual/tr/function.preg-match.php)
- [Python re](https://docs.python.org/3/library/re.html)


## 2. Array vs. List & Dict

PHP'de array tanımlanırken tıpkı bir liste gibi indexi birer birer artan bir liste oluşturabilceğimiz gibi, `key` ve `value` çifti şeklinde değerin girildiği bir array tanımlamakta mümkündür.

``` php
$list = ['a','b','c'];
$dict = array( "foo" => "value1", "bar" => "value2");

print_r($list);
print_r($dict);
# -----------------------------------------------
# Output:
# Array
# (
#     [0] => a
#     [1] => b
#     [2] => c
# )
# Array
# (
#     [foo] => value1
#     [bar] => value2
# )
```
Python'da ise tek bir array kullanımı yerine `list` ve `dict` olmak üzere iki farklı veri yapısı kullanılarak işlemler gerçekleştirilir.

``` python
l = ['a', 'b', 'c']
d = {'foo': 'value1', 'bar': 'value2'}

print(l)
print(d)
# -----------------------------------------------
# Output:
# ['a', 'b', 'c']
# {'foo': 'value1', 'bar': 'value2'}
```

> PHP'den Python'a dönüştüreceğimiz kodların değerlerinin indexlerinin sıralı arttığı durumlarda `list` tanımlamayı, key-value çifti şeklinde tanımlandığı kısımları `dict` olarak tanımlamayı unutmayınız.

**Offical Docs:**

- [PHP Array](https://www.php.net/manual/tr/language.types.array.php)
- [Python Data Structures](https://docs.python.org/3/tutorial/datastructures.html)


## 3. Date Format

İki dil arasındaki en farklı konulardan birisi date ve date format diyebiliriz. PHP built-in fonksiyonu olan `date()` fonksiyonu ile istediğimiz formatta tarih bilgisi alabilmek gerçekten çok kolay.
Söz konusu Python ise beni her zaman biraz zorlamıştır... Python kodunda tarih bilgisi alabilmek için `datetime` modülü yüklenmelidir ve `datetime.datetime` nesnesi kullanılmalıdır.

``` php
$d = date("Y m d");
echo $d;
echo gettype($d);
# -----------------------------------------------
# 2020 08 24
# string
```

PHP'de tek satırda istediğimiz formatta bir tarih çıktısı alabildik.

``` Python
from datetime import datetime

d = datetime.now()
print(d)
print(type(d))
formated_d = d.strftime("%Y %m %d")
print(formated_d)
print(type(formated_d))
# -----------------------------------------------
# <class 'datetime.datetime'>
# '2020 08 24'
# <class 'str'>
```

Python kodu içerisinde ise istediğimiz formatta bir tarih değeri almak PHP koduna göre daha komplex bir yapıya sahip.

**Offical Docs:**

- [PHP Date](https://www.php.net/manual/tr/function.date.php)
- [Python Datetime](https://docs.python.org/3/library/datetime.html)


## 4. Null Check Operator

PHP dilinde en çok kullanılan operatörlerden birisi olan `@` operatörü ile kullanılan değerinde null-check yapılır. Ancak Python dilinde buna karşılık bir operatör bulunmamaktadır.

Örneğini bir sonraki bölümde bulabilirsiniz.


## 5. Single Line Condition Check

PHP dili C dilinin syntax'ını kullanmaktadır. C ve C'nin etkilediği birçok dil gibi PHP içerisinde `?` operatörü ile tek satırlık şart işlemleri gerçekleştirilebilmektedir.

``` php
$is_value_exist = (@$value) ? true : false;
```

Python kodunda ise `?` operatörü kullanılmamaktadır. Bunun yerine aşağıdaki örnekte yer alan yapı kullanılabilir.

``` python
is_value_exist = True if value else False
```

Görüldüğü gibi yazım şeklinde farklılıklar mevcuttur. Aranacak değer `if` statement'in sağında yer alır. Eğer şart sağlanıyorsa `if` statement'in solundaki değer kullanılır.

**Dikkat!**

Python ve PHP dillerindeki bazı `keyword`'ler ve `operatörler` farklı şekilde yazılmaktadır. Aşağıdaki tabloda bazılarını görebilirsiniz.

### **Keywords**

``` python
PHP      -> Python

true     -> True
false    -> False
null     -> None
function -> def
catch    -> except
elseif   -> elif
include  -> import
```

### **Operators**

``` python
PHP  -> Python

&&   -> and
||   -> or
!=   -> not
```


**Docs:**

- [PHP Keywords](https://www.php.net/manual/tr/reserved.keywords.php)
- [Python Keywords](https://www.w3schools.com/python/python_ref_keywords.asp)


Bu yazı dizisinin ilk kısmında kısaca iki dil arasındaki göze çarpan farklılıklara göz attık.

Bir sonraki yazıda görüşmek üzere...
Yazılım ile kalın :blush:

---
Manşet Görseli UnDraw'dan alınmıştır -> [Undraw](https://undraw.co/illustrations)
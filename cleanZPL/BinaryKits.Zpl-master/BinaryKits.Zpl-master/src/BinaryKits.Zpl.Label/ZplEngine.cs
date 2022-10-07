using BinaryKits.Zpl.Label.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryKits.Zpl.Label
{
    public class ZplEngine : List<ZplElementBase>
    {
        /// <summary>
        /// Start an empty engine
        /// </summary>
        public ZplEngine() { }

        /// <summary>
        /// Start an engine with given elements
        /// </summary>
        /// <param name="elements">Zpl elements to be added</param>
        public ZplEngine(IEnumerable<ZplElementBase> elements) : base(elements) { }

        /// <summary>
        /// Output the Zpl string using given context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<string> Render(ZplRenderOptions context)
        {
            var result = new List<string>();

            if (context.AddStartEndFormat)
            {
                result.Add("^XA");
            }

            if (context.AddDefaultLabelHome)
            {
                result.Add("^LH0,0");
            }

            result.Add(context.ChangeInternationalFontEncoding);

            foreach (var element in this.Where(x => x.IsEnabled))
            {
                //Empty line
                if (context.AddEmptyLineBeforeElementStart)
                {
                    result.Add("");
                }

                //Comments
                if (context.DisplayComments)
                {
                    if (element.Comments.Any())
                    {
                        result.Add("^FX");
                        element.Comments.ForEach(x => result.Add("//" + x.Replace("^", "[caret]").Replace("~", "[tilde]")));
                    }
                }

                //Actual element
                if (context.CompressedRendering)
                {
                    result.Add(string.Join("", element.Render(context)));
                }
                else
                {
                    result.AddRange(element.Render(context));
                }
            }

            if (context.AddStartEndFormat)
            {
                result.Add("^XZ");
            }

           // result.Clear();
           //var tempResult = "\u0010CT~~CD,~CC^~CT~\n^XA\n~TA000\n~JSN\n^LT0\n^MNW\n^PON\n^PMN\n^LH0,0\n^JMA\n^PR10,10\n~SD17\n^JUS\n^LRN\n^CI27\n^PA0,1,1,0\n^XZ\n^XA\n^MMT\n^PW531\n^LL886\n^LS0\n^FO352,769^GFA,213,480,6,:Z64:eJy1kDEOwjAMRWNVKgsiF6iaa3SIypUYGSoVbuaj+AhlY0AE+3sojeiIhzfE3/9/JYSdOYRTxT8PlfLeUmcEn9gvqMXGeDMmLKdV0kgtGVeJ3/9MqZvglDy14GmGc3R5QUB6IfK7Z1s7NaY8RkvOyfymPiuX3CllUJIM+rfEzssOxdnbldjtg81n5qvyfDfnRKhIvG3wAXFOPis=:BC2B\n^FO423,765^GFA,201,504,6,:Z64:eJxjYMAB2Bn40EiiAeP///9QSUKA+QCQ4OH/ACRt5H8AyRo5GyD5wUYGSD6wAJKMDyyAbmA8ACELcJAPIKQcSNcDkN6PB0Dm1B+oAJL2jSCT5RkfgN14gFy/MH4Ai0N89R9sDP8PiFKwkD3YfPY/YA5ElRyCzf4ProYZrB7sXYg5EDOh5pMchvjcjAkA90xP/g==:10E7\n^FO467,767^GFA,169,492,6,:Z64:eJxjYMAB2Bn40EgiAOPx//9QSZwqDyCTBdQh/4PtRSIpcTNjA5j8ASb/gzn1H0Ak/3+w/H8IB6IeQkJCqQ5EMINF5MFqwJrZ/4CY9g/ApiGZfADZFga4Gqh6iF6oORAzIeZD7fqHcAPYPRC3gZkQN0NNBpsCABqeTjI=:A4EB\n^FO423,489^GFA,377,1608,6,:Z64:eJzdlEFuwkAMRT1EajaouUCVXINFpFyJZRZVaFdca44yEheYrmCBGPy/Ry0INTSwANWLpyjyfH/bk4icRuEV8yoq22anfK9bZWzflGGhdGHxqvTG5S8MxhqnAs5+eeisfK/sPqHcuICKzstIPJsfoY6M1rqDKR3OeTVmcA4/2gu6S35Ad2GPyWwilLcBXH2Q7HAge3L5V0715phSJcyzSZjt4LDHXlA4CuYfhBuU+meqN/G/eHsha5KZxR5s4FCwQSkSnW/Bbs1WSjCS/uSZ7y0n59tZ08mapm+1cl3zgOYfyPOY/l083X9jpBfevHwlhVdSqp2l8lVH/fJyU3wuD985tl+2azqmmfWn/1tGPF/GEe6nDWo=:85A3\n^FO348,505^GFA,377,1512,6,:Z64:eJzVkzFSwzAQRb/HhTp0ASbmCJQUHnwllyk8iY+mo+gIKlMwLH93J0EkGRCBAhTnFZ7V3y/9NVCvfSJ6OZBRXslhnchnxBV4Qk9u0bNoRkcWdBn4+SMiZ2xfjXvV7t8g7Mg38PKkt+lcW4/89xjJyLSZO3+ae1jBt5r41ux77rn5matdPjmjaW7Yh73Sngzyot2lqBGbwM+XWKXYfPIe7J3OJ0z/7LJbLqRivdfVVNm7HDuWLx3+m+/IfHZ2Ik8hph2O6XhS7rbOsd2tV9Z7Xe3jDOxO3bu2GXhgfVL9oaj+dAjkskTtMg5ULveqmTeqmWMxZmMy4p13rfzu3QarGHQEMKkRLPphYTa14srhd9jg7Q0aFC2Q:2776\n^FO467,483^GFA,413,1644,6,:Z64:eJzdlEFOw0AMRT0EkU1FLlAl1+giaq7UZRYoLSuuNUcZiQsMq3ZRdfD/nkJRQwmwoMKLpyjyfH/bk4icRuEVsyoq22anfKhbZWznyrBQurC4V3rj6hMGY41TAWdfPHTWvld2j1BuXEBF5+VCXJsfoY5crPULpnT4yC/jBs7hR3tBd8kP6C7sMZnnCOVtANcbkh0OZE+upvK73hxTqoR5NgmzHRz22AsKR8H8g3CDUr9P9Uf8L97uyJpkZrEHGzgUbFCKROdbsHtiKyUYSX/yzPeWk/PtrOlkTdO3WrmueUDzf0iEg89bfpXzAttZCr7TsR1N3IXlZ5qCqS25fbsJVdrI8YaMxRXtaCzO7xt7OzbEKy/VzlL5qmOV8rwKn8vDW45542/YdEwz60+4569VSwpw:4F43\n^FO180,74^GFA,289,676,4,:Z64:eJydks0KgkAUhU8M6K5yL/gKA4LL9FEEwbatolXjqueyVa/hqrXLFpEdHRtHwQovfMLcuX9nrgAtIScSE7fk50HOwOrGswB8cqArhTG3qTsG2zK/1hwZmDMhl8CeZD1hBXiVDk9lR1tDNJWu/bX+Dwv63p85bdgKqtZY5vDsqP96mHlkamY3tL6YGjbFkLBOgB39Uf8OI+iLeBdMchbqgHqSF+9zxoVjZMZ6d/a5zGv3Sr2bzNqVPW+rw0+Wa1P8lZoC4lpBUIhLcQ7FzDHd/Rssjl5o:9763\n^FO136,38^GFA,377,1230,6,:Z64:eJy9lDFuwzAMRakqKDfnAobdY3Qomh4rQ4F6y5YT5DABOmTLGXSEjBoKqflfhq2msuEhLYcH4+ublETaIlk8ktqRF9AGSh+U2iR95ZIDIxbUewovEKSm8Up1JN0rKYaJMfxkMR5YCqu2J3aihy1Y12BDVtUM7S6xAU/IYD6xcXm73NRTqUrsdxhZPUZPXW44EQtPetdgFZPdmx4h912WMm030jjq6d2Ms2FOPGPGv4nXJzA8o2TA6NkzZsDu0d/5SVB67J7+8zsysJtpnGVznK5a6KNxmEPb1QMn5ud6883odL/qtt1E0f+e/8VnN3zTpB5HXtvaJyuljQM1/TeSqxmfNQweS3/rhzwpZ59/wbfzDSmQo1c=:BE1B\n^FO119,601^GFA,997,1984,8,:Z64:eJx9lcFu00AQhmezVWwJV7bbS5GAhPICqeDAoSpF3PsMfgWkHjhY4JILp0ioD8CrWAIBbxHTShVHn6ocopj5ZyaJnaRspX717uy/M7OzUyIeriAZrlbSwjgo6BIMSroHn2XuGvwwdCnsK2NBOm9b44YoN24MnWqUztbdsfEES8xUl9xXW5rKEtEFyXm6pGOKD4el9XnBUpdUr32UOdEhRrFht+26jrydopzcWI3hAnlXCCPKZD6k4SqV2Odm+refK/t9ognsPblvsHca3y7eMvc80Y3tMx3xn3WXcayuEv4Hp0ZzIvquDN+S3Ct4r5T7A9MWkZ9ry2s7DsRFGqf6wafmdmg7L6RX0yYt55uqw+7wxb7k35/vP3QPVkfO6kjzcGw8MaZ6r6gjsZ8aL7bkYrmSCcXwH5fRs/1uF2+tkG5olXhvOha/6lnSHkrGcr6T3K6ecFnXq/f0ea3z0/hL9VR6YcwfOOLSPu5N59rqM1U7vL+xzv9Xz5nejuLrPVH2z3QqekFvSmb4nD5VvgDLEbcVLrUiiTJlOESJliNmMGE7po94H1PGklv5kNHUXZ7L78RqNeEfJa3ZKHf1IbJ8CNvNhug16oe3vNrcivdyYkxpWutWoW7NUXqJXUZiyTyS5OY0qMEFOLalVt4dxaIz5nU5tzoYoi801WESiVGaxMLHyaMrlPJ+Hf5BKe9V4Xt710+NMdjUoUcpc76c9pXuiC2PA+OBZf/QUpgmfqbncX/pRXweXzK3Jj4vRUvjc1K0Lr78l+CwDO7ALPM/wHJE0voK6PG+IumdoqUVSf8jWhrXhSSB6+EKzDLQcz1E79gr/ueEvoZ35uYUmJOxOReCLCbfbCfrcAr2cGrA8YGe4xNnOT7oeY4P/XnA8WF2zvFBoub4VG+vYjvR532izzq8PhW9ORN6Az1X3q/2+1L8IpKkhed0hi8mXgFINdcNODLWwRddd79RUmz/F5yhAlXviP4BeNnrdg==:35DE\n^FO41,88^GFA,377,1308,12,:Z64:eJztk01uxSAMhP2URZY5AkfJ0YjUg5WblCNkySJiao8N6o9e+6qqu44i5ROCMRgGTagbmuAHvBhfj3HCIStOyf0Ow3lT3idX5YOcPnBCIe/8V3JW3ia7lzFYoxnfoN6rbkh5MV7QjTf9KeNI9nU7J6py5jmAUxk8t44kPOkIGS94dt7hqmKbdxXlLfgQK0x1byTlbXL2tmbySU7TZhgd0dVhE4vjRrigBFs1Gcrh+K/f668y4uy5MK1fcVztyIgpfeJC9oy85Ur2vPi7APPSfPvMy+XbYV66l+0xYGU9I4WWF9kT0WjWOOJ56XyKkZFTHssIovr3edln7N7nZZ02sTg6zIBVGe0bkeKkMUVmv14BdCGhow==:1B7B\n^FO268,642^GFA,409,1656,8,:Z64:eJzNU0GOgzAMdJYDt6YPWJWPoPKxVeFp3PYbPIEjhwjXMWMalq4Wuq1US9aA4tjjsUO0w05/4H/uPooFM48b8FX179kH8CB+ROyn+GX6dsqroUyxIx8iDlQNEQNxJ5iz40bQ8feoyWoOihX3ip5bxUzDbv8FzhEv95mSfB75ix71pD745GPCz/gaf+sn7e+nnRZevgVu3Y/d5lTGqHuA3qozSb5Wj1kDZMhFHEjeUxb19nKo2BB9RRQvIw/x80rD5/nLdCDsnZ/3874OTJXqMECHjlxIdPit/zLRaA+u+kf+Wfd2qm9zEX46J+Nr/Od+rD/rd6Nt1V3e3xTfAzGMHGtGNcp7vHfiwSpMaAtJFa5mNs/z8h/nFm/3kc/yo57VNz4zP+P72F5dAbGlA7g=:E862\n^FT491,347^BXB,5,200,0,0,1,_,1\n^FH\\^FDeyIzIjoiMjdiOGVkODA2Yzc3OTlkMGE3ZWU5M2YyMzYzMWI4NzE0MDVlNzMxNWZlZTkzNTQwMzhmMjhmYjlkZGM1MTZmNSJ9^FS\n^FO41,539^GFA,593,3100,10,:Z64:eJzt1TFygzAQBdBlKEiRyR5BRyE3gxv4StwgV+AGcUnBoGi1u19gFKdx4cyYgnmF+Qhp/5g4EhHHUW4vvfQ4HW7hVlNFM1EPDRp1NXVxIYpb1kpNVpvunSmOnNVEuYqWgyjrelQvmqFJFEQj5dftJcH5dWkRZNGqtEyNyzGTiu1RecR+du+Kfk1PqsUOkXbaoFjTCM0nDWnHXKspQHKgrmjq8kGPuvMTNKuoaIiLqY+rKUApkHGoF1ULpcAv6NtOcPAxyFNyqwB1FbVQA1HRAPVQgLiibjdXPmuNDXGOtkmUbSCPXk8KclA3gb9Ee+Bf0RthrbXoe2vdR5cVbidJ9BN04b4e3JRYaUq/68y5KYymtJWmHDpzbsq+M1vpzKXSmXNTIiZ7iPc6w/HUlDfoA51hdIbRGZ4G19xDwXX1pvDi08erjzOvPn28+cyxTNRisnE+KOg4J1ngQRadZE2R/8EibYpIA1WbSZsi0sCkd22KSNea9KGBSVw05qaIpqDjlbfBxTpeug1Tlm5DVuOSjzelQFcPhYo6qK2ogahogHooQAx1UAs1EBV9VkT/StG68pT6AT8wBnM=:5747\n^FO202,689^GFA,305,1280,8,:Z64:eJy9kkEOgyAQRYeSlKU3kIs02qP0GF0Y5WgcxSOwdNGUOsOMJgajRtOJ4UUDw38gwHaVK9xV75PsYvzmeLZvpm44PAHuyBd7jvNb5Dh8kDXogCx84ZEmWIdUQ8uBh8Q6JKZpAJbfO2ozzde8XvpJf9lP9pc8kk/yZkpyJxr3V67d1+6y6fxUjEQTI51L0Rs6F+sUTagAGuSDnpnvpf9JXuUD7KPZx4SiJy+nkhfApV7L9dJX9pF9JYfkkpxT7s3i39EbR308e/Wa7yvv1dCnQ15VzqecfQb2CQd8fj1vk5o=:4D32\n^FO49,310^GFA,913,2430,18,:Z64:eJyV1U1u1TAQAGCnlvCuvgCqr/EkFr5WV8+WOECP8I7S7NjBEXDFosumYkEkgoeZ8b8fomC10ssnz3iSeJwbMQ81Xd8K7Ucx3qyjWK/DKE6ojX8skKOjkHsKhxS9HPSHQwKkaJoQ6YeuslEqngIpHy1kMaWtQsXg/wJNPM/TJCGXI6ggaBJ5PQ7K9Rxck2Kp5eBE3RKne3KmBSnOZi91ikjPxnyFfqR76gfOUqNs6Ta7sYvzMomEdRQsOLhRNOx2FAuHeUscxFF2TPv/ssEsvskhND+dXqyfxYlR8liv1/pTPfNd/Mu9G9gn0bBNgs95EgF+Wot3999lu5LAm2Ol59D1FQpvbF9FwUqb34leaP+dB8Fdm9qqSMDL3HpFqBm3JhLFib6FJdZphzYnMWtXDosOXTnYghu2SJxE7sckC3Tl4JvGK9hmcaMc1PLxSko791JOHMxxCNFexCi5gUlqk6VCXcRnFJ8S+Sa5zzjORmzxx3BvyhsUJt7YyyXc3ymTzwITlTUP4XSnZJO2PbkkA+p4SzSJj5hZrq9ZHlDgdCV7L49xFovy0okCB8Y/d0In0Lj68msWIX/Mol8wT3CU5ynLlyops7CPLJ+V9HmOo3qCjU3OLC7WqOVg+RBrZrnb6RnKbRYd9PwMV/Ue89zbmpm2Mon5JH0SJ7I8lznnJnnOkeSkIdfMnZUlVcidRVF3kKNUEF3Tc8B6Jb5EmY9pdSeK6G8pc6yivic5spxu1U+OknubAyzpm5WFa1ahRTmek79ZXT2p0XuxouXRAcqnOoviHX50AiTLXiV3iiqf6NxNPpeDAqXj0keirr6K/JHIEqncVE4WTxGxycG3XY5Bk3qWDsa9yJY2VymnnrOxShnnUk4deDD6UYzXIwgt3k0yX9P4DdfmWho=:0CE2\n^PQ1,,,Y\n^XZ\n";
           // List<string> idList = tempResult.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
           // result = idList;
            return result;
        }

        public string ToZplString(ZplRenderOptions context)
        {
            return string.Join("\n", Render(context));
        }

        /// <summary>
        /// Add raw Zpl fragment
        /// </summary>
        /// <param name="rawZplCode"></param>
        public void AddRawZplCode(string rawZplCode)
        {
            Add(new ZplRaw(rawZplCode));
        }

        /// <summary>
        /// Convert a char to be Hex value, 2 letters
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string MapToHexadecimalValue(char input)
        {
            return Convert.ToByte(input).ToString("X2");
        }
    }
}

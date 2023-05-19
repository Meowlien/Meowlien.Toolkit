using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowlien.Toolkit.Logging {

    public static class LogFormatter {

        // 包裹格式
        public class Package {
            
            public string LogStr { get => log.ToString(); }

            public string FormatStr { get; } = "{0,20} : {1,-10}\n";

            protected struct Data {

                public string Title { get; set; }
                public Dictionary<string, string> SectionList { get; private set; }
                public string? CurrSection { get => currSection; }

                private int index = 0;
                private string? currSection;
                private string sectionException = "Exception";

                // 構建式
                public Data() {
                    Title = "Default";
                    SectionList = new();
                    currSection = null;
                    CreateSection(sectionException);
                }

                // 創建 Section
                public void CreateSection(string section) {
                    currSection = section;
                    SectionList.Add(section, "");
                    index++;
                }

                // 插入資料
                public bool Insert(string section, string data, bool cover = false) {
                    // 索引(section)是否已存在
                    if (SectionList.ContainsKey(section) == true) {
                        if (cover) SectionList[section] = data;     // 覆蓋資料
                        else SectionList[section] += data;          // 追加資料
                        return true;
                    }

                    HandleException(
                        $"!!!!: Cannot Found The Section >> {section} >>\n" +
                        $"{data}"
                    );
                    return false;   // 追加失敗
                }

                // 追加資料
                public bool Append(string data) {
                    // 如果沒有索引值
                    if (currSection == null) {
                        HandleException(data);
                        return false;   // 追加失敗
                    }
                    Insert(currSection, data);
                    return true;    // 追加成功
                }

                // 無視 Section
                public void Push(string data) {
                    currSection = $"{++index}";
                    SectionList.Add(currSection, data);
                }

                // BUG: 移除單元
                public void Remove(string section) {
                    // BUG: 會有找不到 CurrSection 的問題
                    if(SectionList.ContainsKey(section) == true) {
                        SectionList.Remove(section);
                        currSection = null;
                    }
                }

                // 支援處理例外狀況
                private void HandleException(string data) {
                    SectionList[sectionException] += $"<Exception>\n{data}\n</Exception>\n";
                }

                // 清空所有内容
                public void Clear() {
                    SectionList.Clear();
                    currSection = null;
                }

                // 格式化為字串
                public new string ToString() {
                    string str = "";
                    foreach (var section in SectionList) {
                        str += section.Value;
                    }
                    return str;
                }
            }





            protected Data log = new();

            // 建議 >> 實體變數名稱： logPkg
            public Package(string title) {
                log.Title = title;
                log.Push($"\n>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Start\n");
                log.CreateSection("Title");

                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("UTC");
                var timeStr = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo).ToString("yy-MM-dd HH:mm:ss.ffff");
                log.Append($"|> [{timeStr}] *** {title} ***\n");
            }

            public void CreateSection(string section) {
                log.CreateSection(section);
                log.Push($"\n|> {section}\n");
            }

            // 單獨資料：沒有 Section
            public void PushString(string data) {
                log.Push(data);
            }
            #region 追加資料：於前項 Section 繼續追加
            public bool Append(string data) {
                return log.Append(data);
            }
            public bool Append(string key, object? value) {
                return log.Append(string.Format(FormatStr, $"{key}", $"{value}"));
            }
            public bool Append(KeyValuePair<string, object?> item) {
                return log.Append(string.Format(FormatStr, $"{item.Key}", $"{item.Value}"));
            }
            #endregion

            public void CreateAndPushItems(string section, ICollection<KeyValuePair<string, object?>> dataList) {
                CreateSection(section);
                foreach (var item in dataList) {
                    // if(++i == 3) logStr += $">>>>>>>>>>>>>>>>>>>>>> Values\n";
                    log.Append(string.Format(FormatStr, $"{item.Key}", $"{item.Value}"));
                }
            }

            public new string ToString() {
                log.Push($"\n>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> End\n");
                return log.ToString();
            }

            public void Clear() {
                log.Clear();
            }




        }



    }

}

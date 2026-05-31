# TasteDiary 录屏脚本（精简版，~12 分钟）

---

## 结构：先 PC，后手机，不穿插

| 段 | 时长 | 平台 |
|---|------|------|
| 1. 开场 + UI/UX | 2.5 min | 🪟 Windows |
| 2. 代码质量 + 部署 + GitHub | 2 min | 🪟 Windows |
| 3. 功能 + 验证 | 2.5 min | 📱 Android |
| 4. 硬件 + 无障碍 | 3.5 min | 📱 Android |
| 5. 总结 | 0.5 min | 🪟 Windows |

**总时长：约 11 分钟**

---

## 录制前准备

- 🪟 电脑：打开 TasteDiary Windows 版 → Win+G 录屏
- 🪟 VS2022：载入项目，终端就绪
- 🪟 浏览器：打开 GitHub 仓库页面
- 📱 手机：打开 TasteDiary → 屏幕录制 → 开发者选项打开「显示触摸操作」

---

---

# 【第一部分：Windows 端】

---

## 1. 开场 + UI/UX（2.5 min）🪟

---

> 🎤 **Hello, my name is [你的名字]. This is my screencast for 6G6Z0014 Mobile Computing. I've built a cross-platform Food and Drink app called TasteDiary using .NET MAUI, C#, and XAML. I'll first demo on Windows, then switch to a physical Android device for the hardware features.**

🖱️ 展示 Windows 版主页面，看着镜头说。

---

> 🎤 **The app uses three-tab Shell navigation. Let me click through: Foods, Hardware, Settings.**

🖱️ 鼠标依次点击 3 个标签 → 回到 Foods。

---

> 🎤 **The warm earth-tone colour palette: cream background, deep red headings, gold calorie badges, green nutrition data. All colours use AppThemeBinding — they adapt automatically to light and dark mode.**

🖱️ 鼠标在列表中缓慢移动，指向不同颜色区域。

---

> 🎤 **The main banner shows the app name TasteDiary, a tagline, and three badges. Below is a search bar and a green Add button.**

🖱️ 鼠标从顶部 Banner 下移到搜索框和 Add 按钮。

---

> 🎤 **Each food card shows a thumbnail, name, calorie badge, description, macronutrient breakdown — protein, carbs, fat — category label, and a Details button. Cards have rounded borders and soft shadows.**

🖱️ 鼠标在一个卡片上逐项指出。

---

> 🎤 **Theme switching — Settings tab, theme picker. Let me select Dark. The entire app switches instantly. Light. System default. This uses Application.Current.UserAppTheme.**

🖱️ 切到 Settings → 选 Dark → 展示 → 切回 Foods 看暗色效果 → 切回 Settings → 选 Light。

---

---

## 2. 代码质量 + 部署 + GitHub（2 min）🪟

---

> 🎤 **Now let me show the code. Every public member across 13 source files has XML documentation — over 170 lines of /// comments.**

🖱️ 打开 VS2022 → 打开 `FoodItem.cs` → 滚动展示 `/// <summary>`。

---

> 🎤 **Project structure: Models folder, Services folder, and individual page files with their XAML markup. Clear separation of concerns.**

🖱️ 展示 Solution Explorer。

---

> 🎤 **Reusable helpers — SetStatus on HardwarePage is called by all six hardware handlers.**

🖱️ 打开 `HardwarePage.xaml.cs` → 指向 `SetStatus()`。

---

> 🎤 **Roslyn analysis — AnalysisMode All, GenerateDocumentationFile enabled.**

🖱️ 打开 `TasteDiary.csproj` → 指向配置行。

> 🎤 **Let me build to show zero warnings.**

🖱️ 终端跑 `dotnet build -c Release` → 指向 "0 个警告 0 个错误"。

---

> 🎤 **For deployment — the csproj targets four platforms: Android, iOS, Mac Catalyst, and Windows. One codebase, all platforms.**

🖱️ 指向 `<TargetFrameworks>` 那一行。

---

> 🎤 **For GitHub — 10 commits across multiple days, not a single push. The history shows progressive development: scaffold, features, Android fixes, code quality, README.**

🖱️ 浏览器打开 GitHub → 展示 commit 列表 → 滚动到 README 评分对照表。

---

---

# 【第二部分：Android 手机端】

---

## 3. 功能 + 验证（2.5 min）📱

---

🖱️ 画面切到手机录屏。

> 🎤 **Now on the Android device. Let me demonstrate core functionality.**

🖱️ 展示手机主页面。

---

> 🎤 **Search — I type "noodle". Real-time filtering across name, category, description, and tags.**

🖱️ 搜索框输入 `noodle` → 列表过滤为 1 条 → 删除 → 恢复全部。

---

> 🎤 **Pull to refresh — I pull down on the list to reload.**

🖱️ 手指向下拉触发刷新动画。

---

> 🎤 **Details — I tap Details on Braised Beef Noodle Soup. Shell navigation passes the item ID as a query parameter. The detail page shows full nutrition: name, calories, protein, carbs, fat, description, allergens.**

🖱️ 点详情 → 手指从上往下滑展示全部内容。

> 🎤 **"Read summary" uses Text-to-Speech to read the nutrition aloud.**

🖱️ 点 Read summary → 听 3-4 秒 → 点 Stop reading。

> 🎤 **"Vibration reminder" triggers vibration and haptic feedback, with a confirmation alert.**

🖱️ 点 Vibration reminder → 震动 + 弹窗 → OK → 返回。

---

> 🎤 **Add Record — I fill in a new item. Name "Test", category Snack, calories 200, protein 10, carbs 20, fat 5. Save.**

🖱️ 点 Add → 快速填写 → 点 Save → 弹窗 → OK。

---

> 🎤 **Validation — saving an empty form triggers sequential checks with user-friendly messages and vibration.**

🖱️ 再点 Add → 不填任何东西点 Save → 展示错误 + 震动。

> 🎤 **"Please enter a food or drink name" — every field is validated.**

🖱️ 只填名字 → 再点 Save → 展示分类必选。

> 🎤 **Negative numbers are caught too — "Please enter a valid non-negative number."**

🖱️ 填分类和描述 → Calories 填 `-100` → 展示数字错误。

---

---

## 4. 硬件 + 无障碍（3.5 min）📱

---

> 🎤 **Now the six mobile hardware APIs, all on the Hardware tab.**

🖱️ 切换到 Hardware 标签。

---

> 🎤 **1. Camera — MediaPicker.CapturePhotoAsync. I'll take a photo.**

🖱️ 点 Take photo → 拍照 → 指照片 → 指状态文字 "Food photo captured successfully"。

---

> 🎤 **2. GPS and Geocoding — Geolocation.GetLocationAsync then reverse geocode to a readable address.**

🖱️ 点 Get location → 等待 → 指经纬度数字 → 指地址文字。

---

> 🎤 **3. Text-to-Speech — reads the help text aloud.**

🖱️ 点 Read help → 听 3-4 秒 → 点 Stop。

---

> 🎤 **4. Vibration and Haptic Feedback — since these can't be seen on video, the counter provides visible proof.**

🖱️ 点 Haptic feedback → 再点 → 再点 → 指 "Haptic feedback tests: 3"。

> 🎤 **Each tap calls Vibration.Vibrate for 450ms and HapticFeedback.Perform with long-press. Six hardware APIs demonstrated — the top mark threshold is four.**

🖱️ 手指在按钮间划过。

---

> 🎤 **For accessibility — I follow WCAG. Large text mode scales all text by 1.22×, applied recursively to every Label, Button, Entry, and SearchBar. The setting persists across pages.**

🖱️ 切到 Settings → 打开 Large text → 指预览变大 → 切到 Foods → 指列表文字变大 → 切回 Settings → 关闭。

---

> 🎤 **All buttons have SemanticProperties.Hint. All state changes call SemanticScreenReader.Announce. Dark mode via AppThemeBinding supports low-light users. Images have SemanticProperties.Description for TalkBack.**

🖱️ 口播即可。

---

---

# 【第三部分：总结】

---

## 5. 总结（0.5 min）🪟 Windows

---

🖱️ 画面切回电脑，看着镜头。

> 🎤 **To summarise — TasteDiary covers all seven marking criteria. Clean XAML UI with themes and accessibility, 30%. Six hardware APIs, 20%. Full CRUD functionality with search and navigation, 20%. Form validation and graceful error handling throughout, 10%. Complete XML documentation, clean architecture, zero Roslyn warnings, 10%. Multi-platform targets, 5%. Regular GitHub commits with comprehensive README, 5%. Thank you for watching.**

🖱️ 微笑，停止录屏。

---

## 录制清单

| 视频段 | 录什么 | 工具 |
|--------|--------|------|
| 第一部分（段 1-2） | Windows App + VS + GitHub | Win+G，一段到底 |
| 第二部分（段 3-4） | 手机 App | 手机屏幕录制，一段到底 |
| 第三部分（段 5） | 电脑镜头/屏幕 | Win+G |

**剪辑：** 三段按顺序拼接即可，无需画中画，无需来回切。

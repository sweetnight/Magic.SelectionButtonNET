# Magic.SelectionButtonNET

WinForms helper library untuk mengelola ToolStripDropDownButton sebagai Single Selection dan Multiple Selection dengan state yang konsisten.

Library ini hanya bertanggung jawab pada:

* pembuatan menu dropdown
* manajemen state selection
* pengiriman hasil selection melalui event

Library ini TIDAK menangani:

* database
* business logic
* filtering data
* side effects aplikasi

---

## Fitur

* Single selection dropdown
* Multiple selection dropdown
* Opsi "All" (opsional)
* Event-driven
* Reusable
* Tidak bergantung pada struktur database

---

## Instalasi

Tambahkan project atau DLL Magic.SelectionButtonNET ke solusi WinForms Anda.

```csharp
using Magic.SelectionButtonNET;
```

---

## Single Selection

Event `SelectedMenuItemEvent` mengirimkan object `SelectedMenuItemEventArgs` dengan properti berikut:

| Property | Type | Description |
| ------- | ---- | ----------- |
| FirstTime | bool | `true` jika event berasal dari proses inisialisasi, `false` jika berasal dari interaksi user |
| SelectedItemId | int | ID item terpilih (Single Selection) |
| SelectedItemIds | List<int> | Daftar ID terpilih (Multiple Selection) |
| SelectedText | string | Text item terpilih |


### Class

```csharp
Single
```

### Input Properties

| Property                | Description                  |
| ----------------------- | ---------------------------- |
| ToolStripDropDownButton | Target dropdown UI           |
| ItemData                | Data pilihan (ID → Text)     |
| SelectedItemId          | ID terpilih (input & output) |

---

### Event

```csharp
event Action<SelectedMenuItemEventArgs> SelectedMenuItemEvent
```

Event dipanggil setiap kali selection berubah.

---

---- | ---- | ----------- |
| FirstTime | bool | Menandakan event berasal dari proses inisialisasi (`true`) atau dari interaksi user (`false`) |

Event akan otomatis dipanggil saat:

```csharp
InitializeToolStripMenu()
```

dengan nilai:

```csharp
FirstTime = true
```

---

### Cara Penggunaan yang Benar

```csharp
var single = new Single
{
    ToolStripDropDownButton = campaignsButton,
    ItemData = new Dictionary<int, string>
    {
        {1, "Campaign A"},
        {2, "Campaign B"},
        {3, "Campaign C"}
    },
    SelectedItemId = 2
};

single.SelectedMenuItemEvent += e =>
{
    // Abaikan event inisialisasi
    if (e.FirstTime) return;

    Console.WriteLine($"User selected ID: {e.SelectedItemId}");
};

single.InitializeToolStripMenu();
```

---

## Multiple Selection

### Class

```csharp
Multiple
```

### Input Properties

| Property                | Description                  |
| ----------------------- | ---------------------------- |
| ToolStripDropDownButton | Target dropdown UI           |
| ItemData                | Data pilihan (ID → Text)     |
| SelectedItemIds         | ID terpilih (input & output) |
| UseAll                  | Aktifkan menu "All"          |

---

### Aturan Selection

Jika UseAll = true dan All dipilih:

```csharp
SelectedItemIds = new List<int> { -1 };
```

Jika semua item dipilih manual:

```csharp
SelectedItemIds = new List<int> { -1 };
```

Jika sebagian item dipilih:

```csharp
SelectedItemIds = new List<int> { 1, 3, 5 };
```

---

### Event

```csharp
event Action<SelectedMenuItemEventArgs> SelectedMenuItemEvent
```

---

### Contoh Penggunaan

```csharp
var multiple = new Multiple
{
    ToolStripDropDownButton = groupButton,
    ItemData = new Dictionary<int, string>
    {
        {1, "Group A"},
        {2, "Group B"},
        {3, "Group C"}
    },
    SelectedItemIds = new List<int> { 1, 3 },
    UseAll = true
};

multiple.SelectedMenuItemEvent += e =>
{
    if (e.SelectedItemIds.Contains(-1))
        Console.WriteLine("All selected");
    else
        Console.WriteLine(string.Join(", ", e.SelectedItemIds));
};

multiple.InitializeToolStripMenu();
```

---

## Design Contract

* Event `SelectedMenuItemEvent` selalu dipanggil satu kali saat proses inisialisasi pada class `Single` dengan `FirstTime = true`
* Event berikutnya selalu berasal dari interaksi user dengan `FirstTime = false`


* Library hanya mengatur UI dan state selection
* Tidak menyimpan data eksternal
* Semua hasil selection dikirim lewat event
* Aman digunakan ulang oleh developer lain
* Event `SelectedMenuItemEvent` **selalu dipanggil satu kali saat inisialisasi** pada class `Single`
* Developer **wajib memeriksa `FirstTime`** jika hanya ingin merespon interaksi user

---

## Catatan Penting

* Semua input property harus diset sebelum memanggil `InitializeToolStripMenu()`
* Jangan menaruh logic database di dalam class library ini

---

## Lisensi

Internal / Private Library

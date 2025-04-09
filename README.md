# 📦 LibroTrack - Inventory Management Android App

&#x20;&#x20;

LibroTrack is a modern and intuitive Android application built to streamline inventory management using the latest Android technologies like Jetpack Compose, Room, Kotlin Flow, and MVVM architecture. Whether you're managing stock for a small shop or tracking personal items, LibroTrack provides a fast, offline-first, and user-friendly experience.

---

## 🧭 Application Flow

The flow of the LibroTrack application ensures smooth transitions between features with clearly defined inputs, outputs, and data handling at each stage. Below is a comprehensive step-by-step flow of how the user interacts with the app and how data is processed across the screens:

### 🏠 Home Screen

- **Entry Point**: The application launches into the Home screen.
- **Functionality**:
  - Lists all inventory items using Jetpack Compose’s `LazyColumn`.
  - Each item card shows: `Item Name`, `Quantity`, and `Price`.
  - Tapping a card opens the **Item Detail** screen.
  - A Floating Action Button (FAB) at the bottom right navigates to the **Add Item** screen.
- **Data Source**: Items are fetched via `BookDao.getAllBooks()` using a `Flow`, and are auto-updated in real time.
- **Search Input**: A search bar filters the list using `searchBooks(query)` DAO method.

### ➕ Add Item Screen

- **Navigation**: Launched from the FAB on the Home screen.
- **Input Fields**:
  - `Item ID` (String)
  - `Name` (String)
  - `Category` (String)
  - `Quantity` (Int)
  - `Price` (Double)
- **Validation**:
  - All fields are required.
  - Quantity must be ≥ 0, price must be ≥ 0.
- **Submit Action**:
  - On successful validation, a new `Book` object is created.
  - Inserted into Room DB using `bookDao.insert(book)`.
  - Snackbar displays "Item Added Successfully".
  - Redirects back to the **Home Screen**.

### ✏️ Edit Item Screen

- **Navigation**: Accessed via tapping an item or from the Detail screen.
- **Prefilled Fields**: Shows current values of `ID`, `Name`, `Category`, `Quantity`, and `Price`.
- **Editable Inputs**: Same as Add screen with real-time validation.
- **Submit Action**:
  - Updated `Book` object is passed to `bookDao.update(book)`.
  - Snackbar shows "Item Updated Successfully".
  - User is redirected to the **Item Detail Screen** to confirm changes.

### 📄 Item Detail Screen

- **Access**: Tapping any item card on the Home screen or after editing.
- **Data Displayed**:
  - All item fields: `ID`, `Name`, `Category`, `Quantity`, `Price`.
- **Options**:
  - **Edit**: Navigates to Edit screen with item ID.
  - **Delete**: Shows confirmation and proceeds with deletion.

### ❌ Delete Item

- **Triggered From**: Either Detail or Edit screen.
- **Flow**:
  - Calls `bookDao.delete(book)`.
  - A snackbar displays "Item Deleted".
  - User is returned to the **Home Screen**, list updates automatically due to Flow observation.

### 🔍 Search Functionality

- **Location**: Top of Home screen.
- **Input**: Accepts partial or full strings of `Item Name` or `Item ID`.
- **Backend**: Calls `bookDao.searchBooks(query: String)` which is a `Flow<List<Book>>`.
- **UI Update**: As query changes, recomposition updates the displayed list instantly.

### ⚙️ User Preferences

- **Stored Using**: Jetpack DataStore.
- **Settings Tracked**:
  - List/Grid View mode
  - Theme preference (if applicable)
- **Persistence**: Settings remain even after app restarts.
- **Integration**: PreferencesManager exposes them as Flows and they are observed in UI layer.

---

## 🧰 Tech Stack

| Layer            | Tech Used                    |
| ---------------- | ---------------------------- |
| UI               | Jetpack Compose, Material3   |
| Architecture     | MVVM (ViewModel, Repository) |
| Local Storage    | Room (SQLite)                |
| Reactive Updates | Kotlin Flows                 |
| Settings Storage | DataStore Preferences        |

---

## 🚀 Getting Started

### Prerequisites

- Android Studio Giraffe or later
- Android SDK 33+

### Setup Instructions

```bash
# Clone the repository
git clone https://github.com/your-username/librotrack.git

# Open in Android Studio
# Let Gradle sync and then Run the app on emulator/device
```

---

## ✨ Features

- 📋 Inventory listing with card layout
- ➕ Add new items with form validation
- ✏️ Edit existing item details
- ❌ Delete unwanted items with confirmation
- 📄 View detailed information for each item
- 🔍 Real-time search filter by name or ID
- ⚙️ Persistent UI preferences using DataStore
- 📡 Fully offline-capable with Room persistence
- 🔔 Snackbar-based success/error notifications

---

## 📸 Screenshots

> *(Include these images in your GitHub repo)*

- Home Screen
- Add Item Screen
- Edit Item View
- Item Details
- Search Functionality
- Snackbar Notifications

---

## 🧠 Project Structure

```bash
├── data
│   ├── Book.kt
│   ├── BookDao.kt
│   ├── BookDatabase.kt
│   ├── BookRepository.kt
│   └── Converters.kt
├── ui
│   ├── MainActivity.kt
│   ├── BookViewModel.kt
│   ├── Screens (AddEdit, List, Detail)
├── utils
│   └── PreferencesManager.kt
└── AndroidManifest.xml
```

---

## ✅ Contributions

Feel free to fork this project, submit pull requests, or raise issues for bugs or feature suggestions.

---

## 📌 License

This project is licensed under the [MIT License](LICENSE).

---

## 🙋‍♂️ Contact

For any queries or collaboration opportunities:

- 📧 Email: [youremail@example.com](mailto\:youremail@example.com)
- 🐦 Twitter: [@yourhandle](https://twitter.com/yourhandle)
- 🌐 GitHub: [your-username](https://github.com/your-username)

---

> Built with ❤️ using Jetpack Compose


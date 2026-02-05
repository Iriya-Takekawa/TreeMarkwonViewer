# TomboViewer - .NET MAUI Markdown/Text Manager

このリポジトリには、Windows/Android対応のマークダウン/テキストファイル管理アプリケーション「Tombo」の.NET MAUI実装が含まれています。

## プロジェクト構造

```
TomboViewer/
├── Core/           # ドメインモデル（FsNode, PrefsData）
├── Engine/         # ビジネスロジック（FileOps, TreeConstructor, PrefsManager）
├── UI/             # ビューモデル（MainScreenVm, SettingsScreenVm）
├── Infra/          # インフラ（TreeLineCanvas, Converters）
└── README.md       # プロジェクトドキュメント
```

## 実装済みコンポーネント

### ✅ データ構造層 (Core/)
- **FsNode**: 配列ベースの階層構造ノード - `FsNode[]`で子を管理
- **PrefsData**: アプリケーション設定データモデル

### ✅ ビジネスロジック層 (Engine/)
- **FileOps**: ファイル読み書き、タイムスタンプ付き新規作成
- **TreeConstructor**: ディレクトリ構造の再帰的ツリー構築
- **PrefsManager**: JSON形式の設定永続化と外部ツール起動

### ✅ プレゼンテーション層 (UI/)
- **MainScreenVm**: メイン画面のビューモデル（ツリー表示、ファイル選択）
- **SettingsScreenVm**: 設定画面のビューモデル（編集モード、フォルダ設定）

### ✅ インフラストラクチャ層 (Infra/)
- **TreeLineCanvas**: カスタムGraphicsViewによるツリーライン描画
- **Converters**: データバインディング用の5種類のコンバーター

## 独自の設計パターン

### 配列ベースのツリー構造
`List<T>`の代わりに`FsNode[]`を使用し、メモリ効率とアクセス速度を最適化

### 再帰的フラット化アルゴリズム
展開されたノードのみを配列結合により効率的にフラット化

### アクションベース通知
標準の`INotifyPropertyChanged`ではなく、カスタム`Action`デリゲートで変更通知

## 主要機能

- 📁 階層フォルダツリー表示（カスタム描画による視覚化）
- 📄 マークダウン/テキストファイルのプレビュー
- ➕ 新規ファイル作成（`yyyyMMdd_HHmmss_タイトル.md`形式）
- ⚙️ 設定管理（JSONファイル永続化）
- 🔧 外部エディタ/ツール連携（%file%, %path%プレースホルダー）

## ビルド方法

```bash
# 必要: .NET 8.0 SDK + MAUI workload
dotnet workload install maui

# Windows向けビルド
dotnet build -f net8.0-windows

# Android向けビルド  
dotnet build -f net8.0-android
```

## 技術スタック
- .NET 8.0
- .NET MAUI
- C# 12
- System.Text.Json

## 実装状況

✅ コアロジック完成（990行以上のオリジナルC#コード）  
⏳ XAMLビュー実装待ち  
⏳ アプリケーションシェル設定待ち  

詳細は`TomboViewer/README.md`を参照してください。
# 🚀 GuruguruRocket

## 🎮 プロジェクト概要
**GuruguruRocket** はUnityを用いて開発したミニゲームです。プレイヤーはロケットを回転操作し、障害物を避けながらゴールを目指します。

## 🛠使用技術
- Unity（2022.3）
- DoTween
- C#

## リポジトリ構成
```
Assets/
└── Scripts/
    ├── RocketController.cs（ロケットの操作処理）
    ├── ObstacleManager.cs（障害物生成の管理）
    └── GameManager.cs（ゲーム全体の進行制御）

ProjectSettings/
└── Unityのバージョンや設定ファイル
```

※本リポジトリにはコードレビュー用として、主にScriptsフォルダおよびプロジェクト設定のみを含めています。

## コードレビューのポイント
特に以下の点を注目してレビューいただければと思います。

- `RocketController.cs`
  - 直感的操作を実現するための挙動設計
  - 操作性とレスポンスの向上を考えた工夫

- `ObstacleManager.cs`
  - 障害物生成アルゴリズムの設計
  - ゲーム難易度調整の柔軟性

- `GameManager.cs`
  - ゲーム全体の制御フロー
  - コードのメンテナンス性や拡張性への配慮

## ゲーム内容・画面イメージ
実際のゲームの画面や詳細な説明は以下のリンクからご確認いただけます。

[🔗 GuruguruRocket ゲーム詳細（Notionページ）](https://picturesque-kayak-ac4.notion.site/195281634a1680678c77ceda4c0cddf1?pvs=4)

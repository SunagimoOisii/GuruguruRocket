# GuruguruRocket　🚀

## プロジェクト概要
**GuruguruRocket** はUnityで開発したゲームです。プレイヤーはロケットを回転操作し、障害物を避けながらゴールを目指します。

実際のゲームの画面や詳細な説明は以下のリンクからご確認いただけます。
[🔗 GuruguruRocket ゲーム詳細（Notionページ）](https://picturesque-kayak-ac4.notion.site/195281634a1680678c77ceda4c0cddf1?pvs=4)

## 使用技術
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

## 必要なアセットについて
本リポジトリのスクリプトでは、以下のアセットを利用しているものが含まれています。
- **DOTween**：アニメーション処理に使用
- **Addressables(Unity標準パッケージ)**：アセットの動的読み込み・管理機能で使用

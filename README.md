# SkillPilot

AIを活用して、求職中のエンジニアの転職活動を支援するWebサービスです。求人情報をAIで分析し、
自分の保有スキルとの一致率・不足しているスキル・学習ロードマップを可視化します。

> 🚧 開発中です。バックエンド(ASP.NET Core Web API)のMVPは完成し、フロントエンド(React)を実装中です。

---

## 目次

- [プロジェクト概要](#プロジェクト概要)
- [スクリーンショット](#スクリーンショット)
- [使用技術](#使用技術)
- [アーキテクチャ](#アーキテクチャ)
- [ディレクトリ構成](#ディレクトリ構成)
- [セットアップ](#セットアップ)
- [Dockerでの起動(PostgreSQL)](#dockerでの起動postgresql)
- [マイグレーション手順](#マイグレーション手順)
- [Claude API設定方法](#claude-api設定方法)
- [開発ドキュメント](#開発ドキュメント)
- [開発状況](#開発状況)

---

## プロジェクト概要

**コンセプト**: AIで転職活動を支援するサービス
**対象ユーザー**: 転職活動中のエンジニア

求人票を貼り付けてAI分析を実行すると、以下が得られます。

- 求人が求める必須/歓迎スキルの一覧
- 自分の保有スキルとの一致率
- 不足しているスキル
- 不足スキルを埋めるための週単位の学習ロードマップ

開発の背景・要件定義の詳細は [`docs/requirements.md`](docs/requirements.md) を参照してください。

## スクリーンショット

<!--
  フロントエンド実装完了後、各画面のスクリーンショットをここに追加してください。
  画像ファイルは docs/screenshots/ 配下に置くことを想定しています。
  例:
  ### ダッシュボード
  ![ダッシュボード](docs/screenshots/dashboard.png)

  ### 分析結果
  ![分析結果](docs/screenshots/analysis-result.png)

  ### 学習ロードマップ
  ![学習ロードマップ](docs/screenshots/learning-roadmap.png)
-->

_(準備中)_

## 使用技術

| 領域 | 技術 |
|---|---|
| Backend | ASP.NET Core (.NET 10) Web API |
| ORM | Entity Framework Core + Npgsql |
| Database | PostgreSQL 17 |
| 認証 | JWT(httpOnly Cookie方式) |
| API仕様 | Swagger / OpenAPI(Swashbuckle) |
| Frontend | React + TypeScript(実装中) |
| AI | Claude API(Anthropic Messages API) |
| Infra | Docker(ローカル開発用DB) |

## アーキテクチャ

バックエンドはClean Architectureに近い4層構成(Domain / Application / Infrastructure / WebApi)を
採用しています。依存の方向は常に内側(Domain)に向かい、Domain層は他のどの層にも依存しません。

```
WebApi (Controllers, Program.cs)
  ↓ 参照
Infrastructure (EF Core, Repository実装, JWT/パスワードハッシュ, Claude APIクライアント)
  ↓ 参照
Application (UseCase, Result型, DTO, Repository/AIの抽象)
  ↓ 参照
Domain (Entity, ValueObject, Enum, Domain Service) — 何にも依存しない
```

設計判断の詳細・SOLID原則との対応・依存関係図(Mermaid)は
[`docs/architecture.md`](docs/architecture.md) にまとめています。DBスキーマは
[`docs/db.md`](docs/db.md)、API仕様は [`docs/api.md`](docs/api.md) を参照してください。

## ディレクトリ構成

```
skill-pilot/
├── backend/
│   ├── src/
│   │   ├── SkillPilot.Domain/          # エンティティ・ValueObject・Enum
│   │   ├── SkillPilot.Application/     # UseCase・Result型・DTO
│   │   ├── SkillPilot.Infrastructure/  # EF Core・Repository・認証・AI連携
│   │   └── SkillPilot.WebApi/          # Controller・DI登録(Program.cs)
│   └── tests/                          # 各層に対応するテストプロジェクト
├── frontend/                            # React + TypeScript(実装中)
├── docs/                                # 設計ドキュメント一式
├── docker-compose.yml                   # ローカル開発用PostgreSQL
└── .editorconfig
```

## セットアップ

### 前提条件

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Node.js(フロントエンド実装後に必要)
- Claude APIキー([Anthropic Console](https://console.anthropic.com/)で発行)

### バックエンドの起動手順

```bash
git clone https://github.com/leanlab842-art/skill-pilot.git
cd skill-pilot

# 1. ローカルDBを起動(下記「Dockerでの起動」参照)
docker compose up -d db

# 2. 開発用シークレットを設定(下記「Claude API設定方法」も参照)
cd backend/src/SkillPilot.WebApi
dotnet user-secrets set "Jwt:SigningKey" "$(openssl rand -base64 48)"

# 3. マイグレーションを適用(下記「マイグレーション手順」参照)
cd ../..
dotnet ef database update \
  --project src/SkillPilot.Infrastructure \
  --startup-project src/SkillPilot.WebApi

# 4. APIを起動
dotnet run --project src/SkillPilot.WebApi --launch-profile https
```

起動後、`https://localhost:7191/swagger` でSwagger UIから全APIの動作確認ができます
(ポート番号は `backend/src/SkillPilot.WebApi/Properties/launchSettings.json` を参照)。

### フロントエンドの起動手順

実装中です。実装完了後、以下の手順に更新します。

```bash
cd frontend
npm install
npm run dev
```

## Dockerでの起動(PostgreSQL)

現時点ではローカル開発用にPostgreSQLのみをDocker化しています(`docker-compose.yml`)。

```bash
# 起動
docker compose up -d db

# 状態確認
docker compose ps

# 停止
docker compose down
```

接続情報(ローカル開発用、`appsettings.Development.json`と対応):

| 項目 | 値 |
|---|---|
| Host | localhost |
| Port | 5433(ローカルにインストール済みのPostgreSQLとの衝突を避けるため5432ではない) |
| Database | skillpilot_dev |
| Username | postgres |
| Password | postgres |

## マイグレーション手順

EF Core CLI(`dotnet-ef`)はリポジトリのローカルツールとして導入済みです(`backend/dotnet-tools.json`)。

```bash
cd backend

# 初回のみ: ローカルツールを復元
dotnet tool restore

# マイグレーションを作成する(モデルを変更した場合)
dotnet ef migrations add <名前> \
  --project src/SkillPilot.Infrastructure \
  --startup-project src/SkillPilot.WebApi \
  --output-dir Persistence/Migrations

# DBへ適用する
dotnet ef database update \
  --project src/SkillPilot.Infrastructure \
  --startup-project src/SkillPilot.WebApi
```

## Claude API設定方法

求人分析にはClaude API(Anthropic Messages API)を使用します。APIキーは
[Anthropic Console](https://console.anthropic.com/) から発行してください。

ローカル開発では `appsettings.json` に平文で書かず、`dotnet user-secrets` で設定します。

```bash
cd backend/src/SkillPilot.WebApi
dotnet user-secrets set "Ai:Claude:ApiKey" "sk-ant-xxxxxxxx"
dotnet user-secrets set "Ai:Claude:Model" "<使用するモデルID(Anthropicのドキュメントを参照)>"
```

Docker/本番相当環境では環境変数で設定します(`__` はセクション区切り)。

```bash
Ai__Claude__ApiKey=sk-ant-xxxxxxxx
Ai__Claude__Model=claude-opus-4-1-20250805
```

`Ai:Provider` の値(既定は `Claude`)でAIプロバイダを切り替える設計になっています
(詳細は [`docs/architecture.md`](docs/architecture.md) の「AIプロバイダの差し替え設計」を参照)。

## 開発ドキュメント

| ドキュメント | 内容 |
|---|---|
| [`docs/requirements.md`](docs/requirements.md) | 要件定義(MVPスコープ、将来追加) |
| [`docs/db.md`](docs/db.md) | DB設計(ER図、Domain層のクラス設計) |
| [`docs/api.md`](docs/api.md) | API設計(エンドポイント一覧、認証方式) |
| [`docs/architecture.md`](docs/architecture.md) | バックエンドアーキテクチャ(層構成、SOLID対応) |
| [`docs/screen-design.md`](docs/screen-design.md) | 画面設計(画面遷移、ワイヤーフレーム) |
| [`CLAUDE.md`](CLAUDE.md) | Claude Codeでの開発ルール |

## 開発状況

- ✅ 設計フェーズ(要件定義・DB設計・API設計・アーキテクチャ設計)
- ✅ バックエンドMVP(Domain / Application / Infrastructure / WebApi、全APIエンドポイント実装・動作確認済み)
- 🚧 フロントエンド(React + TypeScript)実装中
- ⬜ テストコード整備
- ⬜ 本番相当環境へのデプロイ

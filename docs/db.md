ER図

Users

  │

  │1

  │

  └──────∞ JobAnalysis

```
            │

    ┌───────┴────────┐

    │                │

    │                │

    ▼                ▼
```

SkillResult      LearningRoadmap





各テーブル詳細

Users

---

Id

Name

Email

PasswordHash

CreatedAt

UpdatedAt

役割

- ログイン
- ユーザー管理

JobAnalysis

---

Id

UserId

CompanyName

JobTitle

JobUrl

JobDescription

MatchRate

CreatedAt

役割

分析した求人を保存します。

SkillResult

---

Id

AnalysisId

SkillName

Level

Category

例：AIが分析した結果

Docker

Required

など

LearningRoadmap

---

Id

AnalysisId

Title

Description

Week

Completed

例：Week1

Docker基礎
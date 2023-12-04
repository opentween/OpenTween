#!/bin/sh

# Subversion で管理されていた頃の Tween のコミットログ (SourceForge.JP, CodeRepos) を接続するスクリプト
#
# 実行後は `git blame Tween_svn-last -- Tween/Tween.vb` のように変更履歴を追うことができます

set -eu

git remote add tween_coderepos https://github.com/opentween/Tween_CodeRepos.git

# Tween_CodeRepos を取得 + Tween_v1.1.0.0 などのタグを強制的に上書き
git fetch --force --tags tween_coderepos

# OpenTween のコミットログのうち以下の 2 つを git replace で置換する

# r1643: 3ポスト以上の通知はまとめる
git replace 6a654b6edaa338fc890494c9fa6a19594277b6b2 $(git rev-parse Tween_sourceforge-last^0)

# r1521: 1010リリース
git replace ddbe79b3cfb2baa4e4799a00a2004ba10546aef1 $(git rev-parse Tween_v1.0.1.0^0)

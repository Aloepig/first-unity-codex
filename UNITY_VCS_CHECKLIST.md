# Unity Version Control Checklist

## 1) Unity Editor Settings (required)
- [ ] `Edit > Project Settings > Editor > Version Control` = `Visible Meta Files`
- [ ] `Edit > Project Settings > Editor > Asset Serialization` = `Force Text`
- [ ] `Edit > Preferences > External Tools`에서 사용하는 IDE 설정 확인 (선택)

## 2) What to commit
- [ ] `Assets/` 전체
- [ ] `ProjectSettings/` 전체
- [ ] `Packages/manifest.json`
- [ ] `Packages/packages-lock.json`
- [ ] `*.meta` 파일 포함 여부 확인

## 3) What to ignore
- [ ] `Library/`
- [ ] `Temp/`
- [ ] `Logs/`
- [ ] `obj/`
- [ ] `Build/`, `Builds/`
- [ ] `UserSettings/`

## 4) Scene / Hierarchy management
- [ ] 씬(`*.unity`)은 Hierarchy 정보를 포함하므로 반드시 커밋
- [ ] 프리팹(`*.prefab`) 변경도 함께 커밋
- [ ] 큰 씬 작업 전/후로 자주 커밋해 충돌 범위 최소화

## 5) Team workflow recommendations
- [ ] 한 PR에서 같은 씬을 여러 명이 동시에 대규모 수정하지 않기
- [ ] 씬 충돌 시 YAML 머지 툴(UnityYAMLMerge) 사용 고려
- [ ] 에셋 이동/이름 변경은 Unity Editor 안에서 수행 (`.meta` 참조 유지)

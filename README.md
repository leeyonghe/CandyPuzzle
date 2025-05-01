# Puzzle Game / 퍼즐 게임

A Unity-based puzzle game project. [Debugging in progress]
Unity 기반의 퍼즐 게임 프로젝트입니다. [디버깅중]

핵심방법 : stable-diffusion-webui-docker를 이용해 게임 이미지를 생성해 게임 이미지로 사용

## Project Structure / 프로젝트 구조

- `Assets/` - Contains all game assets including scripts, prefabs, scenes, and resources
  - 스크립트, 프리팹, 씬, 리소스 등 모든 게임 에셋이 포함된 디렉토리
- `ProjectSettings/` - Unity project configuration files
  - Unity 프로젝트 설정 파일
- `Packages/` - Unity package dependencies
  - Unity 패키지 의존성
- `Library/` - Unity internal files (do not modify)
  - Unity 내부 파일 (수정하지 마세요)
- `Logs/` - Project log files
  - 프로젝트 로그 파일

## Requirements / 요구사항

- Unity 2022.3 or later
  - Unity 2022.3 이상
- .NET Framework 4.x
  - .NET Framework 4.x

## Getting Started / 시작하기

1. Clone the repository
   - 저장소를 클론합니다
2. Open the project in Unity by double-clicking `Puzzle.sln`
   - Unity에서 `Puzzle.sln` 파일을 더블클릭하여 프로젝트를 엽니다
3. Open the main scene from the `Assets/Scenes` directory
   - `Assets/Scenes` 디렉토리에서 메인 씬을 엽니다

## Development / 개발

- Main game logic is implemented in C# scripts located in `Assets/Scripts`
  - 주요 게임 로직은 `Assets/Scripts`에 있는 C# 스크립트로 구현되어 있습니다
- Prefabs and game objects are stored in `Assets/Prefabs`
  - 프리팹과 게임 오브젝트는 `Assets/Prefabs`에 저장되어 있습니다
- Scenes are stored in `Assets/Scenes`
  - 씬은 `Assets/Scenes`에 저장되어 있습니다

## License / 라이센스

This project is licensed under the MIT License - see the LICENSE file for details.
이 프로젝트는 MIT 라이센스 하에 배포됩니다. 자세한 내용은 LICENSE 파일을 참조하세요. 

## 빌드 스크립트
/Applications/Unity/Hub/Editor/2022.3.6f1/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/lee/Desktop/CandyPuzzle -executeMethod BuildScript.TerminalBuild -logFile /Users/lee/Desktop/CandyPuzzle/build.log -quit

@echo off
chcp 65001 >nul
title 安装 ClashM Elevated Service

net session >nul 2>&1
if %errorlevel% neq 0 (
    echo 请以管理员身份运行此脚本
    pause
    exit /b 1
)

set SERVICE_NAME=ClashMElevated
:: 直接指向 ClashM.Elevated.exe，不需要中间层
set SERVICE_EXE=%~dp0ClashM.ServiceWrapper.exe

:: 删除旧服务（如果存在）
sc query %SERVICE_NAME% >nul 2>&1
if %errorlevel% equ 0 (
    sc stop %SERVICE_NAME%
    sc delete %SERVICE_NAME%
    timeout /t 2 /nobreak >nul
)

:: 安装服务
sc create %SERVICE_NAME% binPath="%SERVICE_EXE%" start=auto DisplayName="ClashM Elevated Service"
sc description %SERVICE_NAME% "为 ClashM 应用提供特权操作支持"

:: 生成令牌
powershell -Command ^
    "$token = [System.Guid]::NewGuid().ToString('N') + [System.Guid]::NewGuid().ToString('N');" ^
    "reg add HKLM\SOFTWARE\ClashM /v ElevatedToken /t REG_SZ /d $token /f" ^
    "if (-not (Test-Path 'HKLM:\SOFTWARE\ClashM\ClientSid')) {" ^
    "    $sid = [System.Security.Principal.WindowsIdentity]::GetCurrent().User.Value;" ^
    "    reg add HKLM\SOFTWARE\ClashM /v ClientSid /t REG_SZ /d $sid /f" ^
    "}"

:: 启动服务
sc start %SERVICE_NAME%
echo 服务已安装并启动
pause
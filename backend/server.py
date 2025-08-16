from fastapi import FastAPI, HTTPException, BackgroundTasks, WebSocket, WebSocketDisconnect
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
from pydantic import BaseModel
import subprocess
import psutil
import os
import json
import time
import asyncio
from typing import Dict, List, Optional
import uuid
from datetime import datetime
import aiofiles
from pathlib import Path
import re

app = FastAPI(title="Corporate Telegram Tools", version="1.0.0")

# Enable CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Global state management
running_processes: Dict[str, Dict] = {}
websocket_connections: List[WebSocket] = []

# Models
class ScriptExecuteRequest(BaseModel):
    script_name: str
    parameters: Optional[Dict] = {}

class ProcessStatus(BaseModel):
    process_id: str
    script_name: str
    status: str  # running, completed, failed, stopped
    start_time: str
    end_time: Optional[str] = None
    pid: Optional[int] = None
    logs: List[str] = []

# Script configurations
SCRIPT_CONFIGS = {
    "tg_link_collector": {
        "name": "TG Link Collector",
        "description": "Сбор ссылок на Telegram чаты через OKSEARCH",
        "script_path": "/app/1TGlinkV1.0/Сбор ссылок на чаты OKSEARCH.py",
        "work_directory": "/app/1TGlinkV1.0",
        "can_run_background": True
    },
    "filter_not_bot": {
        "name": "Filter Not Bot",
        "description": "Фильтрация ботов из списка чатов",
        "script_path": "/app/3FiltrTGV1.0/ФИЛЬТР НЕ БОТ.py",
        "work_directory": "/app/3FiltrTGV1.0",
        "can_run_background": True
    },
    "remove_duplicates": {
        "name": "Remove Duplicates",
        "description": "Удаление повторяющихся ссылок",
        "script_path": "/app/4POVTORЧЕК/повторные ссылки тг.py", 
        "work_directory": "/app/4POVTORЧЕК",
        "can_run_background": True
    },
    "count_chats": {
        "name": "Count Chats",
        "description": "Подсчет и группировка чатов",
        "script_path": "/app/5ChekLinksHUM/Колич.чатов.py",
        "work_directory": "/app/5ChekLinksHUM",
        "can_run_background": True
    },
    "proxy_checker": {
        "name": "Proxy Checker", 
        "description": "Проверка работоспособности прокси серверов",
        "script_path": "/app/ПРОКСИ ЧЕКЕР/прокси_чек.py",
        "work_directory": "/app/ПРОКСИ ЧЕКЕР",
        "can_run_background": True
    },
    "main_automation": {
        "name": "Main Automation (опционально)",
        "description": "Главный координирующий скрипт (требует Windows .exe файлы)",
        "script_path": "/app/УПравление/ГЛАВА.py",
        "work_directory": "/app/УПравление", 
        "can_run_background": False
    }
}

async def broadcast_message(message: dict):
    """Broadcast message to all connected WebSocket clients"""
    if websocket_connections:
        disconnected = []
        for websocket in websocket_connections:
            try:
                await websocket.send_text(json.dumps(message))
            except:
                disconnected.append(websocket)
        
        # Remove disconnected clients
        for ws in disconnected:
            websocket_connections.remove(ws)

async def monitor_process(process_id: str, process: subprocess.Popen, script_name: str):
    """Monitor process execution and update status"""
    process_info = running_processes[process_id]
    
    try:
        # Wait for process completion
        await asyncio.get_event_loop().run_in_executor(None, process.wait)
        
        if process.returncode == 0:
            process_info["status"] = "completed"
            process_info["logs"].append(f"Process completed successfully with return code {process.returncode}")
        else:
            process_info["status"] = "failed"
            process_info["logs"].append(f"Process failed with return code {process.returncode}")
        
    except Exception as e:
        process_info["status"] = "failed"
        process_info["logs"].append(f"Error monitoring process: {str(e)}")
    
    finally:
        process_info["end_time"] = datetime.now().isoformat()
        
        # Broadcast status update
        await broadcast_message({
            "type": "process_status",
            "process_id": process_id,
            "status": process_info["status"],
            "end_time": process_info["end_time"]
        })

@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    websocket_connections.append(websocket)
    
    try:
        while True:
            # Keep connection alive
            await websocket.receive_text()
    except WebSocketDisconnect:
        websocket_connections.remove(websocket)

@app.get("/")
async def root():
    return {"message": "Corporate Telegram Tools API"}

@app.get("/api/scripts")
async def get_available_scripts():
    """Get list of available scripts"""
    return {"scripts": SCRIPT_CONFIGS}

@app.post("/api/scripts/execute")
async def execute_script(request: ScriptExecuteRequest, background_tasks: BackgroundTasks):
    """Execute a script"""
    script_name = request.script_name
    
    if script_name not in SCRIPT_CONFIGS:
        raise HTTPException(status_code=400, detail="Invalid script name")
    
    config = SCRIPT_CONFIGS[script_name]
    
    # Check if script file exists
    if not os.path.exists(config["script_path"]):
        raise HTTPException(status_code=404, detail=f"Script file not found: {config['script_path']}")
    
    # Generate process ID
    process_id = str(uuid.uuid4())
    
    try:
        # Start process
        process = subprocess.Popen(
            ["python", config["script_path"]],
            cwd=config["work_directory"],
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            universal_newlines=True,
            bufsize=1
        )
        
        # Store process info
        running_processes[process_id] = {
            "process_id": process_id,
            "script_name": script_name,
            "status": "running",
            "start_time": datetime.now().isoformat(),
            "end_time": None,
            "pid": process.pid,
            "logs": [f"Started {config['name']} at {datetime.now()}"]
        }
        
        # Monitor process in background
        if config["can_run_background"]:
            background_tasks.add_task(monitor_process, process_id, process, script_name)
        
        # Broadcast start message
        await broadcast_message({
            "type": "process_started",
            "process_id": process_id,
            "script_name": script_name,
            "start_time": running_processes[process_id]["start_time"]
        })
        
        return {
            "process_id": process_id,
            "status": "started",
            "message": f"Script {script_name} started successfully"
        }
        
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Failed to start script: {str(e)}")

@app.get("/api/processes")
async def get_running_processes():
    """Get list of running processes"""
    return {"processes": list(running_processes.values())}

@app.get("/api/processes/{process_id}")
async def get_process_status(process_id: str):
    """Get status of specific process"""
    if process_id not in running_processes:
        raise HTTPException(status_code=404, detail="Process not found")
    
    return running_processes[process_id]

@app.post("/api/processes/{process_id}/stop")
async def stop_process(process_id: str):
    """Stop a running process"""
    if process_id not in running_processes:
        raise HTTPException(status_code=404, detail="Process not found")
    
    process_info = running_processes[process_id]
    
    if process_info["status"] != "running":
        raise HTTPException(status_code=400, detail="Process is not running")
    
    try:
        # Kill process
        if process_info["pid"]:
            psutil.Process(process_info["pid"]).terminate()
        
        process_info["status"] = "stopped"
        process_info["end_time"] = datetime.now().isoformat()
        process_info["logs"].append("Process stopped by user")
        
        # Broadcast stop message
        await broadcast_message({
            "type": "process_stopped",
            "process_id": process_id
        })
        
        return {"message": "Process stopped successfully"}
        
    except psutil.NoSuchProcess:
        process_info["status"] = "completed"
        return {"message": "Process was already completed"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Failed to stop process: {str(e)}")

@app.get("/api/files")
async def get_files():
    """Get list of files in different directories"""
    directories = {
        "tg_links": "/app/1TGlinkV1.0",
        "online_checker": "/app/2Onlinechat_checker V1.0", 
        "filter_results": "/app/3FiltrTGV1.0",
        "duplicate_check": "/app/4POVTORЧЕК",
        "chat_count": "/app/5ChekLinksHUM",
        "proxy_results": "/app/ПРОКСИ ЧЕКЕР"
    }
    
    result = {}
    
    for name, path in directories.items():
        try:
            if os.path.exists(path):
                files = []
                for item in os.listdir(path):
                    item_path = os.path.join(path, item)
                    if os.path.isfile(item_path):
                        stat = os.stat(item_path)
                        files.append({
                            "name": item,
                            "size": stat.st_size,
                            "modified": datetime.fromtimestamp(stat.st_mtime).isoformat()
                        })
                result[name] = files
            else:
                result[name] = []
        except Exception as e:
            result[name] = {"error": str(e)}
    
    return {"directories": result}

@app.get("/api/files/{directory}/{filename}")
async def get_file_content(directory: str, filename: str):
    """Get content of a specific file"""
    directories = {
        "tg_links": "/app/1TGlinkV1.0",
        "online_checker": "/app/2Onlinechat_checker V1.0",
        "filter_results": "/app/3FiltrTGV1.0", 
        "duplicate_check": "/app/4POVTORЧЕК",
        "chat_count": "/app/5ChekLinksHUM",
        "proxy_results": "/app/ПРОКСИ ЧЕКЕР"
    }
    
    if directory not in directories:
        raise HTTPException(status_code=400, detail="Invalid directory")
    
    file_path = os.path.join(directories[directory], filename)
    
    if not os.path.exists(file_path):
        raise HTTPException(status_code=404, detail="File not found")
    
    try:
        async with aiofiles.open(file_path, 'r', encoding='utf-8') as file:
            content = await file.read()
            
        return {
            "filename": filename,
            "content": content,
            "size": len(content.encode('utf-8'))
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Failed to read file: {str(e)}")

@app.get("/api/system/status")
async def get_system_status():
    """Get system status information"""
    try:
        # Get CPU and memory usage
        cpu_percent = psutil.cpu_percent(interval=1)
        memory = psutil.virtual_memory()
        disk = psutil.disk_usage('/')
        
        return {
            "cpu_percent": cpu_percent,
            "memory": {
                "total": memory.total,
                "available": memory.available,
                "percent": memory.percent
            },
            "disk": {
                "total": disk.total,
                "used": disk.used,
                "free": disk.free,
                "percent": (disk.used / disk.total) * 100
            },
            "active_processes": len([p for p in running_processes.values() if p["status"] == "running"])
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Failed to get system status: {str(e)}")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8001)
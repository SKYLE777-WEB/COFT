import React, { useState, useEffect } from 'react';
import './App.css';
import axios from 'axios';
import {
  Play,
  Square,
  Activity,
  FileText,
  Settings,
  Monitor,
  RefreshCw,
  AlertCircle,
  CheckCircle,
  Clock,
  Cpu,
  HardDrive,
  MemoryStick
} from 'lucide-react';

const API_BASE_URL = process.env.REACT_APP_BACKEND_URL || 'http://localhost:8001';

function App() {
  const [scripts, setScripts] = useState({});
  const [processes, setProcesses] = useState([]);
  const [systemStatus, setSystemStatus] = useState({});
  const [files, setFiles] = useState({});
  const [selectedFile, setSelectedFile] = useState(null);
  const [fileContent, setFileContent] = useState('');
  const [activeTab, setActiveTab] = useState('dashboard');
  const [loading, setLoading] = useState(false);
  const [websocket, setWebsocket] = useState(null);
  const [logs, setLogs] = useState([]);

  useEffect(() => {
    loadInitialData();
    setupWebSocket();
    
    const interval = setInterval(() => {
      loadProcesses();
      loadSystemStatus();
    }, 5000);

    return () => {
      clearInterval(interval);
      if (websocket) {
        websocket.close();
      }
    };
  }, []);

  const setupWebSocket = () => {
    const wsUrl = API_BASE_URL.replace('http', 'ws') + '/ws';
    const ws = new WebSocket(wsUrl);
    
    ws.onopen = () => {
      console.log('WebSocket connected');
      setWebsocket(ws);
    };
    
    ws.onmessage = (event) => {
      const message = JSON.parse(event.data);
      
      if (message.type === 'process_started') {
        addLog(`Process started: ${message.script_name}`);
        loadProcesses();
      } else if (message.type === 'process_status') {
        addLog(`Process ${message.process_id} status: ${message.status}`);
        loadProcesses();
      } else if (message.type === 'process_stopped') {
        addLog(`Process ${message.process_id} stopped`);
        loadProcesses();
      }
    };
    
    ws.onclose = () => {
      console.log('WebSocket disconnected');
      setTimeout(setupWebSocket, 5000); // Reconnect after 5 seconds
    };
  };

  const addLog = (message) => {
    const timestamp = new Date().toLocaleTimeString();
    setLogs(prev => [...prev.slice(-49), `[${timestamp}] ${message}`]);
  };

  const loadInitialData = async () => {
    try {
      await Promise.all([
        loadScripts(),
        loadProcesses(),
        loadSystemStatus(),
        loadFiles()
      ]);
    } catch (error) {
      console.error('Error loading initial data:', error);
    }
  };

  const loadScripts = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/scripts`);
      setScripts(response.data.scripts);
    } catch (error) {
      console.error('Error loading scripts:', error);
    }
  };

  const loadProcesses = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/processes`);
      setProcesses(response.data.processes);
    } catch (error) {
      console.error('Error loading processes:', error);
    }
  };

  const loadSystemStatus = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/system/status`);
      setSystemStatus(response.data);
    } catch (error) {
      console.error('Error loading system status:', error);
    }
  };

  const loadFiles = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/files`);
      setFiles(response.data.directories);
    } catch (error) {
      console.error('Error loading files:', error);
    }
  };

  const executeScript = async (scriptName) => {
    setLoading(true);
    try {
      const response = await axios.post(`${API_BASE_URL}/api/scripts/execute`, {
        script_name: scriptName
      });
      
      addLog(`Starting script: ${scriptName}`);
      await loadProcesses();
    } catch (error) {
      console.error('Error executing script:', error);
      addLog(`Error starting script ${scriptName}: ${error.response?.data?.detail || error.message}`);
    } finally {
      setLoading(false);
    }
  };

  const stopProcess = async (processId) => {
    try {
      await axios.post(`${API_BASE_URL}/api/processes/${processId}/stop`);
      addLog(`Stopping process: ${processId}`);
      await loadProcesses();
    } catch (error) {
      console.error('Error stopping process:', error);
      addLog(`Error stopping process: ${error.response?.data?.detail || error.message}`);
    }
  };

  const loadFileContent = async (directory, filename) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/files/${directory}/${filename}`);
      setFileContent(response.data.content);
      setSelectedFile({ directory, filename });
    } catch (error) {
      console.error('Error loading file content:', error);
      setFileContent(`Error loading file: ${error.response?.data?.detail || error.message}`);
    }
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case 'running':
        return <Clock className="text-blue-500" size={16} />;
      case 'completed':
        return <CheckCircle className="text-green-500" size={16} />;
      case 'failed':
        return <AlertCircle className="text-red-500" size={16} />;
      case 'stopped':
        return <Square className="text-gray-500" size={16} />;
      default:
        return <Activity className="text-gray-400" size={16} />;
    }
  };

  const formatBytes = (bytes) => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="px-6 py-4">
          <div className="flex items-center justify-between">
            <h1 className="text-2xl font-bold text-gray-900">Corporate Telegram Tools</h1>
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2 text-sm text-gray-600">
                <Cpu size={16} />
                <span>CPU: {systemStatus.cpu_percent?.toFixed(1)}%</span>
              </div>
              <div className="flex items-center space-x-2 text-sm text-gray-600">
                <MemoryStick size={16} />
                <span>RAM: {systemStatus.memory?.percent?.toFixed(1)}%</span>
              </div>
              <button
                onClick={loadInitialData}
                className="p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100"
              >
                <RefreshCw size={18} />
              </button>
            </div>
          </div>
        </div>
      </header>

      <div className="flex">
        {/* Sidebar */}
        <aside className="w-64 bg-white shadow-sm h-screen">
          <nav className="p-4">
            <ul className="space-y-2">
              <li>
                <button
                  onClick={() => setActiveTab('dashboard')}
                  className={`w-full flex items-center space-x-3 px-4 py-2 rounded-lg ${
                    activeTab === 'dashboard' ? 'bg-blue-100 text-blue-700' : 'text-gray-700 hover:bg-gray-100'
                  }`}
                >
                  <Monitor size={20} />
                  <span>Dashboard</span>
                </button>
              </li>
              <li>
                <button
                  onClick={() => setActiveTab('scripts')}
                  className={`w-full flex items-center space-x-3 px-4 py-2 rounded-lg ${
                    activeTab === 'scripts' ? 'bg-blue-100 text-blue-700' : 'text-gray-700 hover:bg-gray-100'
                  }`}
                >
                  <Play size={20} />
                  <span>Scripts</span>
                </button>
              </li>
              <li>
                <button
                  onClick={() => setActiveTab('processes')}
                  className={`w-full flex items-center space-x-3 px-4 py-2 rounded-lg ${
                    activeTab === 'processes' ? 'bg-blue-100 text-blue-700' : 'text-gray-700 hover:bg-gray-100'
                  }`}
                >
                  <Activity size={20} />
                  <span>Processes</span>
                  {processes.filter(p => p.status === 'running').length > 0 && (
                    <span className="bg-blue-500 text-white text-xs rounded-full px-2 py-1">
                      {processes.filter(p => p.status === 'running').length}
                    </span>
                  )}
                </button>
              </li>
              <li>
                <button
                  onClick={() => setActiveTab('files')}
                  className={`w-full flex items-center space-x-3 px-4 py-2 rounded-lg ${
                    activeTab === 'files' ? 'bg-blue-100 text-blue-700' : 'text-gray-700 hover:bg-gray-100'
                  }`}
                >
                  <FileText size={20} />
                  <span>Files</span>
                </button>
              </li>
            </ul>
          </nav>
        </aside>

        {/* Main Content */}
        <main className="flex-1 p-6">
          {activeTab === 'dashboard' && (
            <div className="space-y-6">
              <h2 className="text-xl font-semibold text-gray-900">System Dashboard</h2>
              
              {/* System Stats */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div className="bg-white p-6 rounded-lg shadow">
                  <div className="flex items-center">
                    <Cpu className="text-blue-500" size={24} />
                    <div className="ml-4">
                      <p className="text-sm font-medium text-gray-600">CPU Usage</p>
                      <p className="text-2xl font-bold text-gray-900">
                        {systemStatus.cpu_percent?.toFixed(1) || '0'}%
                      </p>
                    </div>
                  </div>
                </div>
                
                <div className="bg-white p-6 rounded-lg shadow">
                  <div className="flex items-center">
                    <MemoryStick className="text-green-500" size={24} />
                    <div className="ml-4">
                      <p className="text-sm font-medium text-gray-600">Memory Usage</p>
                      <p className="text-2xl font-bold text-gray-900">
                        {systemStatus.memory?.percent?.toFixed(1) || '0'}%
                      </p>
                    </div>
                  </div>
                </div>
                
                <div className="bg-white p-6 rounded-lg shadow">
                  <div className="flex items-center">
                    <HardDrive className="text-orange-500" size={24} />
                    <div className="ml-4">
                      <p className="text-sm font-medium text-gray-600">Disk Usage</p>
                      <p className="text-2xl font-bold text-gray-900">
                        {systemStatus.disk?.percent?.toFixed(1) || '0'}%
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              {/* Recent Activity */}
              <div className="bg-white p-6 rounded-lg shadow">
                <h3 className="text-lg font-medium text-gray-900 mb-4">Recent Activity</h3>
                <div className="space-y-2 max-h-60 overflow-y-auto">
                  {logs.length === 0 ? (
                    <p className="text-gray-500 text-sm">No recent activity</p>
                  ) : (
                    logs.slice().reverse().map((log, index) => (
                      <div key={index} className="text-sm text-gray-600 font-mono">
                        {log}
                      </div>
                    ))
                  )}
                </div>
              </div>
            </div>
          )}

          {activeTab === 'scripts' && (
            <div className="space-y-6">
              <h2 className="text-xl font-semibold text-gray-900">Available Scripts</h2>
              
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {Object.entries(scripts).map(([key, script]) => (
                  <div key={key} className="bg-white p-6 rounded-lg shadow">
                    <h3 className="text-lg font-medium text-gray-900 mb-2">{script.name}</h3>
                    <p className="text-gray-600 text-sm mb-4">{script.description}</p>
                    <button
                      onClick={() => executeScript(key)}
                      disabled={loading}
                      className="w-full flex items-center justify-center space-x-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                      <Play size={16} />
                      <span>Execute</span>
                    </button>
                  </div>
                ))}
              </div>
            </div>
          )}

          {activeTab === 'processes' && (
            <div className="space-y-6">
              <h2 className="text-xl font-semibold text-gray-900">Running Processes</h2>
              
              <div className="bg-white shadow overflow-hidden rounded-lg">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Script
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Status
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Started
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Actions
                      </th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {processes.length === 0 ? (
                      <tr>
                        <td colSpan="4" className="px-6 py-4 text-center text-gray-500">
                          No processes running
                        </td>
                      </tr>
                    ) : (
                      processes.map((process) => (
                        <tr key={process.process_id}>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm font-medium text-gray-900">
                              {scripts[process.script_name]?.name || process.script_name}
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="flex items-center space-x-2">
                              {getStatusIcon(process.status)}
                              <span className={`text-sm font-medium ${
                                process.status === 'running' ? 'text-blue-600' :
                                process.status === 'completed' ? 'text-green-600' :
                                process.status === 'failed' ? 'text-red-600' : 'text-gray-600'
                              }`}>
                                {process.status}
                              </span>
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            {new Date(process.start_time).toLocaleString()}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                            {process.status === 'running' && (
                              <button
                                onClick={() => stopProcess(process.process_id)}
                                className="text-red-600 hover:text-red-900 flex items-center space-x-1"
                              >
                                <Square size={16} />
                                <span>Stop</span>
                              </button>
                            )}
                          </td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          {activeTab === 'files' && (
            <div className="space-y-6">
              <h2 className="text-xl font-semibold text-gray-900">File Manager</h2>
              
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* File List */}
                <div className="bg-white p-6 rounded-lg shadow">
                  <h3 className="text-lg font-medium text-gray-900 mb-4">Directories</h3>
                  
                  {Object.entries(files).map(([dirName, dirFiles]) => (
                    <div key={dirName} className="mb-6">
                      <h4 className="text-md font-medium text-gray-800 mb-2 capitalize">
                        {dirName.replace('_', ' ')}
                      </h4>
                      <div className="space-y-1">
                        {Array.isArray(dirFiles) ? (
                          dirFiles.length === 0 ? (
                            <p className="text-gray-500 text-sm">No files</p>
                          ) : (
                            dirFiles.map((file) => (
                              <div
                                key={file.name}
                                className="flex items-center justify-between p-2 rounded hover:bg-gray-50 cursor-pointer"
                                onClick={() => loadFileContent(dirName, file.name)}
                              >
                                <div className="flex items-center space-x-2">
                                  <FileText size={16} className="text-gray-500" />
                                  <span className="text-sm text-gray-900">{file.name}</span>
                                </div>
                                <span className="text-xs text-gray-500">
                                  {formatBytes(file.size)}
                                </span>
                              </div>
                            ))
                          )
                        ) : (
                          <p className="text-red-500 text-sm">Error: {dirFiles.error}</p>
                        )}
                      </div>
                    </div>
                  ))}
                </div>

                {/* File Content */}
                <div className="bg-white p-6 rounded-lg shadow">
                  <h3 className="text-lg font-medium text-gray-900 mb-4">
                    {selectedFile ? `${selectedFile.filename}` : 'Select a file'}
                  </h3>
                  <div className="bg-gray-50 p-4 rounded-lg">
                    <pre className="text-sm text-gray-800 whitespace-pre-wrap overflow-auto max-h-96">
                      {fileContent || 'No file selected'}
                    </pre>
                  </div>
                </div>
              </div>
            </div>
          )}
        </main>
      </div>
    </div>
  );
}

export default App;
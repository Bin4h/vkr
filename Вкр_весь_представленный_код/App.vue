<template>
  <div class="container">
    <div class="files-list">
      <div v-for="(file, index) in uniqueFilesWithoutExtension" :key="index" class="file-item">
        <p>{{ file }}</p>
      </div>
    </div>


    <div class="controls">
      <button @click="openKeyPicker" class="select-btn">
        Выбрать ключ
      </button>
      <input 
        type="file" 
        ref="keyInput" 
        @change="handleKeySelect" 
        style="display: none"
        accept=".txt,.key"
      >
      <div v-if="selectedKeyFile" class="selected-file">
        Название ключевого файла: {{ selectedKeyFile.name }}
      </div>

      <button @click="openFilePicker" class="select-btn">
        Выбрать файл для загрузки
      </button>
      <input 
        type="file" 
        ref="fileInput" 
        @change="handleFileSelect" 
        style="display: none"
      >
      <div v-if="selectedLocalFile" class="selected-file">
        Название файла: {{ selectedLocalFile.name }}
      </div>
      <button 
        @click="sendFilePath" 
        class="upload-btn"
        :disabled="!selectedLocalFile"
      >
        Загрузить файл
      </button>

      <div class="file-selection">
        <button @click="showFileDropdown = !showFileDropdown" class="select-btn">
          Выбрать файл для скачивания
        </button>
        <div v-if="showFileDropdown" class="dropdown">
          <div 
            v-for="(file, index) in uniqueFiles" 
            :key="index" 
            class="dropdown-item"
            @click="selectServerFile(file)"
          >
            {{ file.slice(0, -2) }}
          </div>
        </div>
        <div v-if="selectedServerFile" class="selected-file">
          Выбран файл: {{ selectedServerFile.slice(0, -2) }}
        </div>
      </div>
      
      <button 
        @click="downloadFiles()" 
        class="download-btn"
        :disabled="!selectedServerFile"
      >
        Скачать
      </button>
      
      <div class="radio-group">
        <h3>Выберите метод скачивания:</h3>
        <label v-for="option in options" :key="option.value" class="radio-label">
          <input 
            type="radio" 
            v-model="selectedOption" 
            :value="option.value"
          >
          {{ option.label }}
        </label>
      </div>
      
      <div class="configuration">
        <h3>Настройки RAID:</h3>
        <div class="input-group">
          <label>Метод распределения:</label>
          <select v-model.number="selectedDistributionMethod">
            <option v-for="option in options2" :key="option.value" :value="option.value">
              {{ option.label }}
            </option>
          </select>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';

const state = ref({
  files: []
});

const raidConfig = ref({
  firstSlice: 10, 
  secondSlice: 22  
});

const uniqueFiles = computed(() => {
  return [...new Set(state.value.files)]; 
});

const uniqueFilesWithoutExtension = computed(() => {
  return uniqueFiles.value.map(file => file.slice(0, -2));
});

const options = ref([
  { value: 'option1', label: 'Скачать со всех дисков' },
  { value: 'option2', label: 'Скачать без Яндекс Диска' },
  { value: 'option3', label: 'Скачать без Dropbox' },
  { value: 'option4', label: 'Скачать без Google Drive' },
  { value: 'option5', label: 'Скачать случайным методом' }
]);
const options2 = ref([
  { value: 1, label: 'Конфигурация 1' },
  { value: 2, label: 'Конфигурация 2' },
  { value: 3, label: 'Конфигурация 3' }
]);
const selectedDistributionMethod = ref(1);

const selectedOption = ref('option1');
const selectedServerFile = ref(null);
const selectedLocalFile = ref(null);
const showFileDropdown = ref(false);
const fileInput = ref(null);
const keyInput = ref(null);
const selectedKeyFile = ref(null);
const keyContent = ref('');

const getFilesNames = async () => {
  const response = await fetch(`https://localhost:7229/api/GoogleDrive/files`);
  return await response.json();
}

const selectServerFile = (fileName) => {
  selectedServerFile.value = fileName;
  showFileDropdown.value = false;
}

const openFilePicker = () => {
  fileInput.value.click();
}
const openKeyPicker = () => {
  keyInput.value.click();
};

const handleFileSelect = (event) => {
  const file = event.target.files[0];
  if (file) {
    selectedLocalFile.value = {
      name: file.name,
      path: file.path,
      size: file.size,
      type: file.type,
      lastModified: file.lastModified,
      file: file
    };
    console.log('Selected file object:', selectedLocalFile.value);
  }
}
const keyValues = ref('10,22');
const parseKeyContent = (content) => {
  const lines = content.split('\n');
  return lines[lines.length - 1].trim();
};
// Метод для обработки выбора ключевого файла
const handleKeySelect = async (event) => {
  const file = event.target.files[0];
  if (file) {
    selectedKeyFile.value = {
      name: file.name,
      file: file
    };
    const content = await readFileAsText(file);
    keyValues.value = parseKeyContent(content);
    console.log('Key values:', keyValues.value);
  }
};

// Метод для чтения файла как текста
const readFileAsText = (file) => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = (event) => resolve(event.target.result);
    reader.onerror = (error) => reject(error);
    reader.readAsText(file);
  });
};
// Метод для скачивания файла
const downloadFiles = async () => {
  if (!selectedServerFile.value) return;
  const fileName = selectedServerFile.value.slice(0, -2);
  console.log(fileName);
  
  let responseUrl = ``;
  switch(selectedOption.value) {
    case 'option1':
      responseUrl = `https://localhost:7229/api/RAID5/download/${fileName}?distributionMethod=${selectedDistributionMethod.value}&key=${keyValues.value}`;
      break;
    case 'option2':
      responseUrl = `https://localhost:7229/api/RAID5/downloadwithoutya/${fileName}?distributionMethod=${selectedDistributionMethod.value}&key=${keyValues.value}`;
      break;
    case 'option3':
      responseUrl = `https://localhost:7229/api/RAID5/downloadwithoutgoogle/${fileName}?distributionMethod=${selectedDistributionMethod.value}&key=${keyValues.value}`;
      break;
    case 'option4':
      responseUrl = `https://localhost:7229/api/RAID5/downloadwithoutdropbox/${fileName}?distributionMethod=${selectedDistributionMethod.value}&key=${keyValues.value}`;
      break;
    case 'option5':
      const randomOption = Math.floor(Math.random() * 4) + 1;
      responseUrl = `https://localhost:7229/api/RAID5/${
          randomOption === 1 ? 'download' : 
          randomOption === 2 ? 'downloadwithoutya' :
          randomOption === 3 ? 'downloadwithoutgoogle' : 'downloadwithoutdropbox'
      }/${fileName}?distributionMethod=${selectedDistributionMethod.value}&key=${keyValues.value}`;
      break;
  }
  
  const response = await fetch(responseUrl);
  console.log(response)
  const blob = await response.blob();
  await computeDownload(blob)
  return await response;
}

async function computeDownload(blob) {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement('a');
  const fileName = selectedServerFile.value.slice(0, -2);
  const name = fileName + "";
  link.href = url;
  link.setAttribute('download', name);
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url);
}
// Метод для отправки файла
const sendFilePath = async () => {
  if (!selectedLocalFile.value?.file) {
    alert('Пожалуйста, выберите файл');
    return;
  }
  
  const formData = new FormData();
  formData.append('file', selectedLocalFile.value.file);
  formData.append('distributionMethod', selectedDistributionMethod.value.toString());
  formData.append('key', keyValues.value);

  try {
    const response = await fetch(`https://localhost:7229/api/RAID5/upload`, {
      method: 'POST',
      body: formData
    });

    if (!response.ok) {
      throw new Error(`Ошибка HTTP! статус: ${response.status}`);
    }

    const result = await response.json();
    console.log('Файл успешно загружен:', result);
    alert('Файл успешно загружен');
    
  } catch (error) {
    console.error('Ошибка при загрузке файла:', error);
    alert('Ошибка: ' + error.message);
  }
};

onMounted(async () => {
  state.value.files = await getFilesNames();
  console.log(state.value.files);
});
</script>

<style scoped>
.container {
  display: flex;
  gap: 20px;
  padding: 20px;
}

.files-list {
  flex: 1;
  border: 1px solid #ddd;
  padding: 15px;
  border-radius: 5px;
}

.file-item {
  padding: 8px 0;
  border-bottom: 1px solid #eee;
}

.file-item:last-child {
  border-bottom: none;
}

.controls {
  width: 250px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.file-selection {
  position: relative;
}

.select-btn {
  padding: 10px 15px;
  background-color: #2196F3;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
  width: 100%;
  margin-bottom: 8px;
}

.select-btn:hover {
  background-color: #0b7dda;
}

.dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border: 1px solid #ddd;
  border-radius: 4px;
  box-shadow: 0 2px 5px rgba(0,0,0,0.2);
  z-index: 10;
  max-height: 200px;
  overflow-y: auto;
}

.dropdown-item {
  padding: 8px 15px;
  cursor: pointer;
}

.dropdown-item:hover {
  background-color: #f5f5f5;
}

.selected-file {
  margin-top: 8px;
  padding: 8px;
  background: #f5f5f5;
  border-radius: 4px;
  font-size: 14px;
  margin-bottom: 12px;
  word-break: break-all;
}

.download-btn {
  padding: 10px 15px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
}

.download-btn:hover {
  background-color: #45a049;
}

.download-btn:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.radio-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 5px;
}

.radio-group h3 {
  margin: 0 0 10px 0;
}

.radio-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.radio-label input {
  cursor: pointer;
}
.upload-btn {
  padding: 10px 15px;
  background-color: #ff9800;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
  margin-bottom: 8px;
}

.upload-btn:hover {
  background-color: #e68a00;
}

.upload-btn:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.configuration {
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 5px;
  margin-top: 20px;
}

.input-group {
  margin-bottom: 15px;
}

.input-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

.input-group input,
.input-group select {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
  box-sizing: border-box;
}
</style>
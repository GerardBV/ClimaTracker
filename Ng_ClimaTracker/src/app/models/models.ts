// Auth
export interface RegisterData {
    username: string;
    email: string;
    password: string;
    passwordConfirm: string;
}

export interface LoginData {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
    username: string;
}

// Weather
export interface HourlyDto {
    time: string;
    temperature: number;
}

export interface DailyDto {
    date: string;
    maxTemp: number;
    minTemp: number;
}

export interface WeatherDto {
    city: string;
    currentTemp: number;
    feelsLike: number;
    maxTemp: number;
    minTemp: number;
    condition: string;
    sunrise: string;
    sunset: string;
    hourly: HourlyDto[];
    daily: DailyDto[];
}

export interface Weather {
    id: number;
    city: string;
    isFavorite: boolean;
    userId: string;
}
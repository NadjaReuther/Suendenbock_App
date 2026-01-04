
import { GoogleGenAI } from "@google/genai";
import { PlayerStatus } from "../types";

const ai = new GoogleGenAI({ apiKey: process.env.API_KEY });

/**
 * Service to handle AI-driven game events.
 */
export const geminiService = {
  /**
   * Generates a mysterious message for the player based on their current status.
   */
  async generateStatusCommentary(status: PlayerStatus): Promise<string> {
    try {
      const response = await ai.models.generateContent({
        model: 'gemini-3-flash-preview',
        contents: `You are a medieval narrator in the year 1618. 
        The player has ${status.healthPercent}% health, ${status.openQuests} open quests, and is level ${status.level}.
        Provide a short (10-15 words), atmospheric comment about their current standing in German.`,
      });
      return response.text || "Das Schicksal wartet auf niemanden.";
    } catch (error) {
      console.error("AI Generation Error:", error);
      return "Die Schatten der Vergangenheit flüstern von Gefahr.";
    }
  }
};

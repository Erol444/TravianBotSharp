import { createContext, useContext } from "react";

const VillageContext = createContext();

const useVillage = () => {
	const context = useContext(VillageContext);

	if (!context) {
		throw new Error("useVillage must be used within a <Village />");
	}

	return context;
};

export { VillageContext, useVillage };

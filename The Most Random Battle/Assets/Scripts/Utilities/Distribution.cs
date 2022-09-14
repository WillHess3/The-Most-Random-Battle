using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distribution<T> {

    private readonly List<T> _itemDistribution;

    private readonly List<float> _distributionProportionsCumulative;

    public Distribution(List<T> itemDistribution, List<float> distributionProportions) {
        _itemDistribution = itemDistribution;

        _distributionProportionsCumulative = new List<float>();
        float total = 0;
        for (int i = 0; i < distributionProportions.Count; i++) {
            _distributionProportionsCumulative.Add(total);

            total += distributionProportions[i];
        }
    }

    public T RandomFromDistribution() {
        float randomNumber = Random.value;

        for (int i = _distributionProportionsCumulative.Count - 1; i >= 0; i--) {
            if (randomNumber > _distributionProportionsCumulative[i]) {
                return _itemDistribution[i];
            }
        }

        return _itemDistribution[0];
    }

}
